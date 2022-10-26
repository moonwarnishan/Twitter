
using Microsoft.AspNetCore.Authorization;

namespace SearchServices.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IServices _searchServices;
        public SearchController(IServices searchServices)
        {
            _searchServices = searchServices;
        }
        [HttpGet("{userKeyWord}")]
        [Authorize]
        public async Task<List<UserResponseDto>> getUserResult(string userKeyWord)
        {
            if (userKeyWord == null)
            {
                return null;
            }
            return await _searchServices.Users(userKeyWord);
            
        }
        [HttpGet("{hashKeyWord}")]
        [Authorize]
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
