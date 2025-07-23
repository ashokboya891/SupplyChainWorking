using Microsoft.AspNetCore.SignalR;

namespace SupplyChain.HubsCollection
{
    public class TicketHub:Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            var user = Context.User;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId); // optional per-user group

                // ✅ If user has Admin role, join AdminGroup
                if (user.IsInRole("Admin"))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "AdminGroup");
                    Console.WriteLine($"🟢 Admin {userId} added to AdminGroup");
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, "AdminGroup");
                Console.WriteLine($"🔴 Admin {userId} removed from AdminGroup");
            }

            await base.OnDisconnectedAsync(exception);
        }
        // This will broadcast to all connected clients
        //public async Task BroadcastTicketUpdate(int requestId)
        //{
        //    await Clients.All.SendAsync("ReceiveTicketUpdate", requestId);
        //}

        //public override Task OnConnectedAsync()
        //{
        //    string userId = Context.UserIdentifier;
        //    if (!string.IsNullOrEmpty(userId))
        //    {
        //        Groups.AddToGroupAsync(Context.ConnectionId, userId); // ✅ Group = userId
        //    }
        //    return base.OnConnectedAsync();
        //}

        //public override Task OnDisconnectedAsync(Exception? exception)
        //{
        //    string userId = Context.UserIdentifier;
        //    if (!string.IsNullOrEmpty(userId))
        //    {
        //        Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        //    }
        //    return base.OnDisconnectedAsync(exception);
        //}

    }
}
