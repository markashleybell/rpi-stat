using System.Threading.Tasks;
using core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace rpi_stat_ui.Hubs
{
    public class StatHub : Hub
    {
        private readonly ILogger<StatHub> _logger;

        public StatHub(ILogger<StatHub> logger) =>
            _logger = logger;

        public async Task SendMessage(string message)
        {
            _logger.LogInformation($"{nameof(SendMessage)}: '{message}'");

            await Clients.All.SendAsync(HubEndpoint.ReceiveMessage, message).ConfigureAwait(false);
        }

        public async Task SendTemperature(decimal temperature)
        {
            _logger.LogInformation($"{nameof(SendTemperature)}: {temperature.ToString("0.000")}");

            await Clients.Others.SendAsync(HubEndpoint.ReceiveTemperature, temperature).ConfigureAwait(false);
        }

        public async Task RequestHeatingState(HeatingState state)
        {
            _logger.LogInformation($"{nameof(RequestHeatingState)}: {state}");

            await Clients.Others.SendAsync(HubEndpoint.ReceiveHeatingStateRequest, state).ConfigureAwait(false);
        }

        public async Task ConfirmHeatingState(HeatingState state)
        {
            _logger.LogInformation($"{nameof(ConfirmHeatingState)}: {state}");

            await Clients.Others.SendAsync(HubEndpoint.ReceiveHeatingStateConfirmation, state).ConfigureAwait(false);
        }
    }
}
