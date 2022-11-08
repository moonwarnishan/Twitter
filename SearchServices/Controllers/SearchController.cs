
using Microsoft.AspNetCore.Authorization;

namespace SearchServices.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SearchController : ControllerBase
    {
        private readonly IServices _searchServices;
        public SearchController(IServices searchServices)
        {
            _searchServices = searchServices;
        }
        [HttpGet("{userKeyWord}")]
        public async Task<List<UserDto>> getUserResult(string userKeyWord)
        {
            if (userKeyWord == null)
            {
                return null;
            }
            return await _searchServices.Users(userKeyWord);
            
        }
        [HttpGet("{hashKeyWord}")]
        public async Task<List<HashResponseDto>> getHashResult(string hashKeyWord)
        {
            if (hashKeyWord == null)
            {
                return null;
            }
            return await _searchServices.Hashes('#'+hashKeyWord);
        }

    }
}
