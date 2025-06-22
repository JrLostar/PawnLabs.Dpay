using Microsoft.AspNetCore.SignalR.Client;

namespace PawnLabs.Dpay.Core.Helper
{
    public interface ISocketHelper
    {
        HubConnection Connection { get; set; }

        Task ConnectToHub(string email);
        Task DisconnectFromHub();
    }
}