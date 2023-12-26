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

        public UserService(UserManager<UserApp> userManager)
        {
            _userManager = userManager;
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