using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace ForgeSample.Application.Hubs
{
    /// <summary>
    /// Class uses for SignalR
    /// </summary>
    public class ModelDerivativeHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public string GetConnectionId() { return Context.ConnectionId; }

        /// <summary>
        /// Notify the client that the workitem is complete
        /// </summary>
        public async Task ExtractionFinished(IHubContext<ModelDerivativeHub> context, JObject body)
        {
            string connectionId = body["hook"]["scope"]["workflow"].Value<String>();
            await context.Clients.Client(connectionId).SendAsync("extractionFinished", body);
        }
    }
}
