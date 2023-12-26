using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
    //[Route("api/[controller]/[action]")]
    //[ApiController]
    public class UserController : CustomBaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDTO createUserDTO)
        {
            var result = await _userService.CreateUserAsync(createUserDTO);
            return CreateActionResult(result);
        }

        //bu endpoint token istiyor!!
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserByName()
        {
            //parametre olarak göndermiyoruz get metodunda Token içindeki claimden alıyoruz!!!
            var userName = HttpContext.User.Identity.Name;
            var result = await _userService.GetUserByNameAsync(userName);
            return CreateActionResult(result);
        }
    }
}