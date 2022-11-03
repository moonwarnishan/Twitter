using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TweetRetweetCommentLike.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationServices _notificationServices;
        public NotificationsController(INotificationServices notificationServices)
        {
            _notificationServices=notificationServices;
        }

        [HttpGet("{userName}/{page}")]
        public async Task<List<NotificationDto>> GetNotifications(string userName, int page)
        {
            if (await _notificationServices.GetNotification(userName,page) != null)
            {
                return await _notificationServices.GetNotification(userName,page);
            }
            else
            {
                return null;
            }
        }

    }
}
