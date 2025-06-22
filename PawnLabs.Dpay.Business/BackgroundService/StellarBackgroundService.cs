using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PawnLabs.Dpay.Business.Service;
using PawnLabs.Dpay.Core.Configuration;
using PawnLabs.Dpay.Core.Entity;
using PawnLabs.Dpay.Core.Helper;
using System.Text.Json;

namespace PawnLabs.Dpay.Business.BackgroundService
{
    public class StellarBackgroundService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly StellarConfiguration _stellarConfiguration;

        private readonly IServiceProvider _serviceProvider;

        private CancellationTokenSource? _internalCts;
        private Task? _executingTask;

        private static Guid ProductID = Guid.Empty;
        private static string WalletAddress = string.Empty;

        public StellarBackgroundService(IHttpClientFactory httpClientFactory, IOptions<StellarConfiguration> stellarConfiguration, IServiceProvider serviceProvider)
        {
            _httpClientFactory = httpClientFactory;

            _stellarConfiguration = stellarConfiguration.Value;

            _serviceProvider = serviceProvider;
        }

        public Task StartJobAsync(Guid productID, string walletAddress)
        {
            if (_executingTask != null && !_executingTask.IsCompleted)
                throw new InvalidOperationException("Job is already running.");

            ProductID = productID;
            WalletAddress = walletAddress;
            _internalCts = new CancellationTokenSource();

            _executingTask = Task.Run(() => DoWorkAsync(_internalCts.Token));

            return Task.CompletedTask;
        }

        public async Task StopJobAsync()
        {
            if (_internalCts != null)
            {
                _internalCts.Cancel();
                await (_executingTask ?? Task.CompletedTask);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        private async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            var httpClient = _httpClientFactory.CreateClient();

            int iterationCount = 0;

            while (!cancellationToken.IsCancellationRequested && iterationCount < _stellarConfiguration.IterationCount)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                    var socketHelper = scope.ServiceProvider.GetRequiredService<ISocketHelper>();

                    try
                    {
                        var product = await productService.GetByID(ProductID);

                        var response = await httpClient.GetAsync($"{_stellarConfiguration.HorizonBaseUrl}/accounts/{WalletAddress}/payments", cancellationToken);
                        var json = await response.Content.ReadAsStringAsync(cancellationToken);

                        var doc = JsonDocument.Parse(json);

                        JsonElement records = doc.RootElement
                                                .GetProperty("_embedded")
                                                .GetProperty("records");

                        JsonElement lastRecord = records[records.GetArrayLength() - 1];

                        string createdAt = lastRecord.GetProperty("created_at").GetString();
                        string from = lastRecord.GetProperty("from").GetString();
                        string to = lastRecord.GetProperty("to").GetString();
                        string amount = lastRecord.GetProperty("amount").GetString();

                        var paymentID = await paymentService.Add(new Payment()
                        {
                            Email = product.Email,
                            ProductID = product.ID,
                            BuyerAddress = from,
                            WalletAddress = to,
                            Price = float.Parse(amount),
                            PriceType = product.PriceType,
                            Date = DateTime.Parse(createdAt)
                        });

                        await socketHelper.ConnectToHub(product.Email);
                        await socketHelper.Connection.InvokeAsync("Payment", product.ID);
                        await socketHelper.DisconnectFromHub();
                    }
                    catch (Exception ex)
                    {
                    }
                }

                iterationCount++;

                if (iterationCount >= _stellarConfiguration.IterationCount)
                    break;

                await Task.Delay(TimeSpan.FromSeconds(_stellarConfiguration.Delay), cancellationToken);
            }

            _internalCts?.Cancel();
        }
    }

}
