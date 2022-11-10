namespace RegisterUser.Hub
{
    public class NotificationHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private static readonly ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();
        public void SendChatMessage(string who, NotificationDto message)
        {
            // string name = Context.User;
            Clients.All.SendAsync("Notification", message);
            foreach (var connectionId in _connections.GetConnections(who))
            {
                
            }
        }

        public override Task OnConnectedAsync()
        {
            var name = Context.User;

            _connections.Add("", Context.ConnectionId);

            //return Task.CompletedTask;
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string name = Context.User.Identity.Name;

            _connections.Remove(name, Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
        // public static string GetClaimValue(HttpContext httpContext)
        // {
        //     if (string.IsNullOrEmpty(ClaimTypes.Name)) return null;
        //     var identity = httpContext.User.Identity as ClaimsIdentity;
        //     var valueObj = identity == null ? null : identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
        //     return valueObj == null ? null : valueObj.Value;
        // }


    }
}
        