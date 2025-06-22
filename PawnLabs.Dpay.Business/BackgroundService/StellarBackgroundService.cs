using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PawnLabs.Dpay.Business.Service;
using PawnLabs.Dpay.Core.Configuration;
using PawnLabs.Dpay.Core.Entity;
using PawnLabs.Dpay.Core.Enum;
using PawnLabs.Dpay.Core.Helper;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace PawnLabs.Dpay.Business.BackgroundService
{
    public class StellarBackgroundService : Microsoft.Extensions.Hosting.BackgroundService
    {
        private IHttpClientFactory _httpClientFactory;

        private StellarConfiguration stellarConfiguration;

        private IServiceProvider _serviceProvider;

        public StellarBackgroundService(IHttpClientFactory httpClientFactory, IOptions<StellarConfiguration> stellarConfiguration, IServiceProvider serviceProvider)
        {
            _httpClientFactory = httpClientFactory;

            this.stellarConfiguration = stellarConfiguration.Value;

            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var httpClient = _httpClientFactory.CreateClient();

            string cursor = string.Empty;
            long ledger = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
                    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
                    var configurationService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();

                    var socketHelper = scope.ServiceProvider.GetRequiredService<ISocketHelper>();

                    try
                    {
                        var request = new
                        {
                            jsonrpc = "2.0",
                            id = 1,
                            method = "getEvents",
                            @params = new
                            {
                                startLedger = ledger == 0 ? (await GetLatestLedgerAsync()) - 1000 : ledger,
                                filters = new[]
                                {
                                new
                                {
                                    type = "contract",
                                    contractIds = new[] { stellarConfiguration.ContractID }
                                }
                            },
                                cursor = cursor,
                                limit = 10
                            }
                        };

                        var response = await httpClient.PostAsJsonAsync(stellarConfiguration.SorobanBaseUrl, request, cancellationToken: stoppingToken);
                        var json = await response.Content.ReadAsStringAsync(stoppingToken);

                        var parsed = JsonDocument.Parse(json);
                        var events = parsed.RootElement.GetProperty("result").GetProperty("events");

                        ledger = parsed.RootElement.GetProperty("result").GetProperty("latestLedger").GetInt64();
                        cursor = parsed.RootElement.GetProperty("result").GetProperty("cursor").GetString() ?? string.Empty;

                        foreach (var ev in events.EnumerateArray())
                        {
                            string topicBase64 = ev.GetProperty("topic")[0].GetString() ?? "";
                            string valueBase64 = ev.GetProperty("value").GetString() ?? "";

                            string topic = DecodeSorobanSymbol(topicBase64);
                            string value = DecodeSorobanSymbol(valueBase64);

                            var values = value.Split(",");

                            var fromAddress = values[0];
                            var toAddress = values[1];
                            var productID = values[2];
                            var amount = values[3];
                            var date = values[4];

                            var product = await productService.GetByID(new Guid(productID));

                            if (product == null)
                                continue;

                            if (product.Price != float.Parse(amount))
                                continue;

                            var walletAddress = (string)(await configurationService.GetByType(new Configuration() { Email = product.Email, Type = EnumConfigurationType.WalletAddress })).Value;

                            if (!walletAddress.Equals(toAddress))
                                continue;

                            var paymentID = await paymentService.Add(new Payment()
                            {
                                Email = product.Email,
                                ProductID = product.ID,
                                BuyerAddress = fromAddress,
                                WalletAddress = walletAddress,
                                Price = product.Price,
                                PriceType = product.PriceType,
                                Date = DateTimeOffset.FromUnixTimeSeconds(long.Parse(date)).UtcDateTime
                            });

                            #region Socket

                            await socketHelper.ConnectToHub(product.Email);
                            await socketHelper.Connection.InvokeAsync("Payment", product.ID);
                            await socketHelper.DisconnectFromHub();

                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        break;
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        #region Inner Functions

        private string DecodeSorobanSymbol(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64.Trim());

            string hex = string.Join(" ", bytes.Select(b => b.ToString("x2")));

            List<string> strings = new();
            StringBuilder currentString = new();
            bool inString = false;

            foreach (byte b in bytes)
            {
                if (b >= 32 && b <= 126)
                {
                    currentString.Append((char)b);
                    inString = true;
                }
                else
                {
                    if (inString && currentString.Length > 0)
                    {
                        strings.Add(currentString.ToString());
                        currentString.Clear();
                        inString = false;
                    }
                }
            }
            if (currentString.Length > 0)
                strings.Add(currentString.ToString());

            return string.Join(",", strings);
        }

        private async Task<long> GetLatestLedgerAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();

            var response = await httpClient.GetAsync(stellarConfiguration.HorizonBaseUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var latestLedger = doc.RootElement.GetProperty("history_latest_ledger").GetInt64();

            return latestLedger;
        }

        #endregion
    }
}
