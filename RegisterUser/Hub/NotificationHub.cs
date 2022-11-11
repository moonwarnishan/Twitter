using Microsoft.AspNetCore.Http;

namespace RegisterUser.Hub
{
    public class NotificationHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private static readonly ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();
        public void SendChatMessage(string who, NotificationDto message)
        {
            // string name = Context.User;
            //Clients.All.SendAsync("Notification", message);
            //get http context
            //var userName = Context.GetHttpContext()?.Request.Query["userName"];
            foreach (var connectionId in _connections.GetConnections(who))
            {
                Clients.Client(connectionId).SendAsync("Notification", message);
            }
        }

        public override Task OnConnectedAsync()
        {
            //var userName = Context.GetHttpContext()?.Request.Headers.Authorization;
            var username = Context.GetHttpContext().Request.Query["username"];
            _connections.Add(username, Context.ConnectionId);

            //return Task.CompletedTask;
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var username = Context.GetHttpContext().Request.Query["username"];

            _connections.Remove(username, Context.ConnectionId);
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
        