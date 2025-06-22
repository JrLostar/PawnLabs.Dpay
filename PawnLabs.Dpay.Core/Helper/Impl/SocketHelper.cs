using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using PawnLabs.Dpay.Core.Enum;

namespace PawnLabs.Dpay.Core.Helper.Impl
{
    public class SocketHelper : ISocketHelper
    {
        public HubConnection Connection { get; set; }

        private IConfiguration _configuration;

        private ISecurityHelper _securityHelper;

        public SocketHelper(IConfiguration configuration, ISecurityHelper securityHelper)
        {
            _configuration = configuration;

            _securityHelper = securityHelper;
        }

        public async Task ConnectToHub(string email)
        {
            try
            {
                #region Token

                var token = _securityHelper.GenerateToken(email, EnumApplication.Api);

                if (string.IsNullOrEmpty(token))
                    return;

                #endregion

                Connection = new HubConnectionBuilder().WithUrl(_configuration.GetSection("SocketHubUrl").Value, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                }).Build();

                while (true)
                {
                    if (Connection.State != HubConnectionState.Connected)
                        await Connection.StartAsync();
                    else
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DisconnectFromHub()
        {
            try
            {
                await Connection.StopAsync();
                await Connection.DisposeAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
