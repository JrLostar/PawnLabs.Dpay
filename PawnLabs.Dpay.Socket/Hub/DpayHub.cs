using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PawnLabs.Dpay.Core.Enum;
using System.Collections.Concurrent;

namespace PawnLabs.Dpay.Socket.Hub
{
    [Authorize]
    public class DpayHub : Microsoft.AspNetCore.SignalR.Hub
    {
        #region Claims

        private string Email
        {
            get
            {
                return Context?.User?.Claims.FirstOrDefault(c => c.Type == "Email")?.Value ?? string.Empty;
            }
        }

        private EnumApplication Application
        {
            get
            {
                if(!int.TryParse(Context?.User?.Claims.FirstOrDefault(c => c.Type == "Application")?.Value, out int applicationId) || applicationId <= 0)
                    return EnumApplication.None;

                return (EnumApplication)applicationId;
            }
        }

        #endregion

        #region Static Fields

        // Email, ConnectionId
        private static Lazy<ConcurrentDictionary<string, string>> Emails = new Lazy<ConcurrentDictionary<string, string>>();

        #endregion

        public DpayHub() { }

        #region Override

        public override async Task OnConnectedAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                Context.Abort();
                return;
            }

            if(Application == EnumApplication.Modal)
            {
                if (!Emails.Value.TryAdd(Email, Context.ConnectionId))
                {
                    Context.Abort();
                    return;
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Application == EnumApplication.Modal)
                Emails.Value.TryRemove(Email, out var _);

            await base.OnDisconnectedAsync(exception);
        }

        #endregion

        public async Task Payment(Guid productID)
        {
            if (Application != EnumApplication.Api)
                return;

            if (!Emails.Value.ContainsKey(Email))
                return;

            await Clients.Clients(new List<string>() { Emails.Value[Email] }).SendAsync("Payment", productID);
        }
    }
}
