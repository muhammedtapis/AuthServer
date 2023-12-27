using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using AuthServer.Service.Mapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> _userManager;

        //rol ekleme metodu oluşturcağımız için lazım
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(UserManager<UserApp> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResponseDTO<UserAppDTO>> CreateUserAsync(CreateUserDTO createUserDTO)
        {
            var user = new UserApp() { Email = createUserDTO.Email, UserName = createUserDTO.Username }; //password burda direkt ekleyemeyiz hashlememiz lazım

            var result = await _userManager.CreateAsync(user, createUserDTO.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return ResponseDTO<UserAppDTO>.Fail(StatusCodes.Status400BadRequest, new ErrorDTO(errors, true));
            }

            return ResponseDTO<UserAppDTO>.Success(StatusCodes.Status201Created, ObjectMapper.Mapper.Map<UserAppDTO>(user));
        }

        //kullanıcı adı üzerinden rol ekleme
        public async Task<ResponseDTO<NoContentDTO>> CreateUserRoleAsync(string userName)
        {
            if (!await _roleManager.RoleExistsAsync("admin")) //eğer admin rolu yoksa oluştur.
            {
                await _roleManager.CreateAsync(new IdentityRole() { Name = "admin" });
                await _roleManager.CreateAsync(new IdentityRole() { Name = "manager" });
            }

            var user = await _userManager.FindByNameAsync(userName);
            await _userManager.AddToRoleAsync(user, "admin");
            await _userManager.AddToRoleAsync(user, "manager");

            return ResponseDTO<NoContentDTO>.Success(StatusCodes.Status201Created);
        }

        public async Task<ResponseDTO<UserAppDTO>> GetUserByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return ResponseDTO<UserAppDTO>.Fail(StatusCodes.Status400BadRequest, "Kullanıcı bulunamadı", true);
            }
            return ResponseDTO<UserAppDTO>.Success(StatusCodes.Status200OK, ObjectMapper.Mapper.Map<UserAppDTO>(user));
        }
    }
}