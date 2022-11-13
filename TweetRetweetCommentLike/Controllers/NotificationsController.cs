
namespace TweetRetweetCommentLike.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationServices _notificationServices;
        private readonly ILogger<NotificationsController> _logger;
        public NotificationsController(INotificationServices notificationServices, ILogger<NotificationsController> logger)
        {
            _notificationServices = notificationServices;
            _logger = logger;
        }

        [HttpGet("{userName}/{page}")]
        public async Task<List<NotificationDto>> GetNotifications(string userName, int page)
        {
            if (await _notificationServices.GetNotification(userName,page) != null)
            {
                _logger.LogInformation("Get notifications of {0}", userName);
                return await _notificationServices.GetNotification(userName,page);
            }
            else
            {
                _logger.LogWarning("No notification found for {0}",userName);
                return null;
            }
        }

    }
}
