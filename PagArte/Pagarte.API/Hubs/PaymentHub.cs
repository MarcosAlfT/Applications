using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OpenIddict.Abstractions;

namespace Pagarte.API.Hubs
{
    [Authorize]
    public class PaymentHub : Hub
    {
        // Client subscribes to their own payment updates
        public async Task SubscribeToPayment(string paymentId)
        {
            var clientId = Context.User?.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
            if (string.IsNullOrEmpty(clientId)) return;

            // Join group named by paymentId
            await Groups.AddToGroupAsync(Context.ConnectionId, $"payment_{paymentId}");
        }

        public async Task UnsubscribeFromPayment(string paymentId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"payment_{paymentId}");
        }

        public override async Task OnConnectedAsync()
        {
            var clientId = Context.User?.FindFirst(OpenIddictConstants.Claims.Subject)?.Value;
            if (!string.IsNullOrEmpty(clientId))
            {
                // Each client joins their own group for direct notifications
                await Groups.AddToGroupAsync(Context.ConnectionId, $"client_{clientId}");
            }
            await base.OnConnectedAsync();
        }
    }
}
