using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniApp1.API.Controllers
{
    //program.cs te policy oluşturup eklemen lazım sonra policy burda çağırdık. direkt claim çağıramazsın.
    [Authorize(Roles = "admin", Policy = "AdanaPolicy")]
    [Authorize(Roles = "admin", Policy = "AgePolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStock()
        {
            //tokendaki payloaddan name al
            var userName = HttpContext.User.Identity.Name;
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            return Ok($"Stock => Username : {userName}  UserId : {userIdClaim.Value}");
        }
    }
}