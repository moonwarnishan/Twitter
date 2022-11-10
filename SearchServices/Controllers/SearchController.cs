namespace SearchServices.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly IServices _searchServices;
        private readonly ILogger<SearchController> _logger;
        public SearchController(IServices searchServices, ILogger<SearchController> logger)
        {
            _searchServices = searchServices;
            _logger = logger;
        }

        [HttpGet("{userKeyWord}")]
        public async Task<List<UserDto>> getUserResult(string userKeyWord)
        {
            if (userKeyWord == null)
            {
                return null;
            }
            _logger.LogInformation("Get user result {0}", userKeyWord);
            return await _searchServices.Users(userKeyWord);
            
        }
        [HttpGet("{hashKeyWord}")]
        public async Task<List<HashResponseDto>> getHashResult(string hashKeyWord)
        {
            if (hashKeyWord == null)
            {
                return null;
            }
            _logger.LogInformation("Get Hash result #{0}", hashKeyWord);
            return await _searchServices.Hashes('#'+hashKeyWord);
        }

    }
}
