using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.DTOs;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        [NonAction]
        public IActionResult CreateActionResult<T>(ResponseDTO<T> responseDTO) where T : class
        {
            return new OkObjectResult(responseDTO)
            {
                StatusCode = responseDTO.StatusCode,
            };
        }
    }
}