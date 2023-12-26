using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        //client kontrolü okuması yapcaz clientlogindto için appsettingsten okuycaz direkt o sebeple ctorda IOptions<> okuncak
        private readonly List<Client> _clients;

        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork; //veritabanı commit
        private readonly IGenericRepository<UserRefreshToken> _userRefreshRepository; //refreshtokenları db kaydedicez

        public AuthenticationService(IOptions<List<Client>> optionsClient, ITokenService tokenService, UserManager<UserApp> userManager
            , IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenRepository)
        {
            _clients = optionsClient.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshRepository = userRefreshTokenRepository;
        }

        public async Task<ResponseDTO<TokenDTO>> CreateTokenAsync(LoginDTO loginDto)
        {
            if (loginDto == null)
            {
                throw new ArgumentNullException(nameof(loginDto));
            }

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return ResponseDTO<TokenDTO>.Fail(StatusCodes.Status400BadRequest, "Email veya şifre yanlış", true);
            }
            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password)) // eğer şifre yanlışsa
            {
                return ResponseDTO<TokenDTO>.Fail(StatusCodes.Status400BadRequest, "Email veya şifre yanlış", true);
            }
            var token = _tokenService.CreateToken(user);
            var userRefreshToken = await _userRefreshRepository.Where(x => x.UserId == user.Id).SingleOrDefaultAsync(); //refresh token var mı

            if (userRefreshToken == null) //eğer refreshtoken yoksa oluştur
            {
                await _userRefreshRepository.AddAsync(new UserRefreshToken()
                { UserId = user.Id, Code = token.RefreshToken, Expiration = token.RefreshTokenExpiration });
            }
            userRefreshToken.Code = token.RefreshToken;
            userRefreshToken.Expiration = token.RefreshTokenExpiration;

            await _unitOfWork.CommitAsync();

            return ResponseDTO<TokenDTO>.Success(StatusCodes.Status201Created, token);
        }

        public ResponseDTO<ClientTokenDTO> CreateTokenByClientAsync(ClientLoginDTO clientLoginDto)
        {
            //id ve secret doğru mu apsettingstekilerle aynı mı ona bak
            var client = _clients.SingleOrDefault(x => x.ClientId == clientLoginDto.ClientId && x.ClientSecret == clientLoginDto.ClientSecret);
            if (client == null)
            {
                return ResponseDTO<ClientTokenDTO>.Fail(StatusCodes.Status400BadRequest, "ClientId veya ClientSecret yanlış", true);
            }

            var token = _tokenService.CreateTokenByClient(client);

            return ResponseDTO<ClientTokenDTO>.Success(StatusCodes.Status201Created, token);
        }

        public async Task<ResponseDTO<TokenDTO>> CreateTokenByRefreshTokenAsync(string refreshToken)
        {
            //refresh token var mı önce onu kontrol
            var isExistRefreshToken = _userRefreshRepository.Where(x => x.Code == refreshToken).FirstOrDefault();
            if (isExistRefreshToken == null)
            {
                return ResponseDTO<TokenDTO>.Fail(StatusCodes.Status400BadRequest, "Refresh token bulunamadı", true);
            }
            //refresh token kime ait o kullanıcıyı bul çünkü bu kullanıcıya refresh token üzerinden token üreticez
            var user = await _userManager.FindByIdAsync(isExistRefreshToken.UserId);
            if (user == null)
            {
                return ResponseDTO<TokenDTO>.Fail(StatusCodes.Status400BadRequest, "Kullanıcı bulunamadı", true);
            }

            var tokenDto = _tokenService.CreateToken(user);
            //yeni bir token oluşturduğumuz için var olan refreshtoken güncellememiz lazım
            isExistRefreshToken.Code = tokenDto.RefreshToken;
            isExistRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;

            await _unitOfWork.CommitAsync(); //yapılan güncellemeleri kaydet.

            return ResponseDTO<TokenDTO>.Success(StatusCodes.Status201Created, tokenDto);
        }

        public async Task<ResponseDTO<NoContentDTO>> RevokeRefreshTokenAsync(string refreshToken)
        {
            var isExisRefreshToken = await _userRefreshRepository.Where(x => x.Code == refreshToken).FirstOrDefaultAsync();
            if (isExisRefreshToken == null)
            {
                return ResponseDTO<NoContentDTO>.Fail(StatusCodes.Status400BadRequest, "Refresh token bulunamadı", true);
            }
            _userRefreshRepository.Remove(isExisRefreshToken);

            await _unitOfWork.CommitAsync();

            return ResponseDTO<NoContentDTO>.Success(StatusCodes.Status204NoContent);
        }
    }
}