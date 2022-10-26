using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TweetRetweetCommentLike.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationServices _notificationServices;
        public NotificationsController(INotificationServices notificationServices)
        {
            _notificationServices=notificationServices;
        }

        [HttpGet("{userName}")]
        public async Task<List<NotificationDto>> GetNotifications(string userName)
        {
            if (await _notificationServices.GetNotification(userName) != null)
            {
                return await _notificationServices.GetNotification(userName);
            }
            else
            {
                return null;
            }
        }

    }
}
