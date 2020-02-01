using System.Threading.Tasks;
using core;
using Microsoft.AspNetCore.SignalR;

namespace rpi_stat_ui.Hubs
{
    public class StatHub : Hub
    {
        public async Task SendMessage(string message) =>
            await Clients.All.SendAsync(HubEndpoint.ReceiveMessage, message).ConfigureAwait(false);

        public async Task SendTemperature(decimal temperature) =>
            await Clients.Others.SendAsync(HubEndpoint.ReceiveTemperature, temperature).ConfigureAwait(false);
    }
}
