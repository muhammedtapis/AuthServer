using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    //BU SERVİSTEN APILERIN HABERİ OLMAYACAK BU SERVİSİ AUTHSERVICE KULLANACAK !!!
    public class TokenService : ITokenService
    {
        //nelere ihtiyacımız var ? userapp almışız o sebeple UserManager lazım sonra Program.cs de belirticez
        private readonly UserManager<UserApp> _userManager;

        private readonly CustomTokenOptions _customTokenOptions;
        //burdaki ayarlara göre token oluşturcaz o yüzden lazım constructorda direkt geçmeyceğiz bunu IOptions interface üzerinden geçicez!!!

        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOptions> options)
        {
            _userManager = userManager;
            _customTokenOptions = options.Value; //value su CustomTokenOptions oluyor
        }

        //private refresh token üretecek bir metod tanımlamamız gerekli!

        private string CreateRefreshToken()
        {
            var numberByte = new Byte[32];
            using var random = RandomNumberGenerator.Create(); //bize random değer ğretecek
            random.GetBytes(numberByte);  //üretilen random değerin bytlerını al yukarıdaki numbrByte a aktar.

            return Convert.ToBase64String(numberByte);
        }

        //private claim oluşturma metodu sadece burada kullanılcak.

        private IEnumerable<Claim> GetClaims(UserApp userApp, List<string> audiences)
        {
            //kullanıcının hangi bilgileri claim olarka oluşturulsun onu belirtiyoruz,payloada eklencek claimler
            var userList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,userApp.Id), //parametre olarak gelen kullanıcının idsini nameIdentifier olarak bir claim oluştur
                new Claim(JwtRegisteredClaimNames.Email,userApp.Email),
                new Claim(ClaimTypes.Name,userApp.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            //tokenin kendisi ile alakalı claimler
            //her bir audience için tek tek dön ve audience claimi oluşturup userlist ekle audience hangi API lere erişebilir o demek
            userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));

            return userList;
        }

        //client için claim oluşturma bu metod sadece burada kullanılcak.

        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            var claims = new List<Claim>();
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
            new Claim(JwtRegisteredClaimNames.Sub, client.ClientId.ToString()); //bu token kimin için oluşturuluyıor clientId si için.

            return claims;
        }

        public TokenDTO CreateToken(UserApp user)
        {
            //token ömrünü belirle şuanki zamanı al ve appsettingsten gelen Accesstokenexpiration kadar dk ekle.
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_customTokenOptions.AccessTokenExpiration);

            var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(_customTokenOptions.RefreshTokenExpiration);

            //imzayı al security key
            var securityKey = SignService.GetSymmetricSecurityKey(_customTokenOptions.SecurityKey);
            //imzayı oluştur
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //token constructoru doldur içinde neler olcak ne bilgiler var yaz daha sonra handler oluşturcak bu bilgilerle
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
                (
                    issuer: _customTokenOptions.Issuer,
                    expires: accessTokenExpiration,
                    notBefore: DateTime.Now,
                    claims: GetClaims(user, _customTokenOptions.Audience), //tokenda buluncak claimler
                    signingCredentials: signingCredentials
                );

            var handler = new JwtSecurityTokenHandler(); //tokenı oluşturcak yer bruası

            var token = handler.WriteToken(jwtSecurityToken);

            var tokenDto = new TokenDTO
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration
            };
            return tokenDto;
        }

        public ClientTokenDTO CreateTokenByClient(Client client)
        {
            //token ömrünü belirle şuanki zamanı al ve appsettingsten gelen Accesstokenexpiration kadar dk ekle.
            var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_customTokenOptions.AccessTokenExpiration);

            //imzayı al security key
            var securityKey = SignService.GetSymmetricSecurityKey(_customTokenOptions.SecurityKey);
            //imzayı oluştur
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //token constructoru doldur içinde neler olcak ne bilgiler var yaz daha sonra handler oluşturcak bu bilgilerle
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken
                (
                    issuer: _customTokenOptions.Issuer,
                    expires: accessTokenExpiration,
                    notBefore: DateTime.Now,
                    claims: GetClaimsByClient(client), //tokenda buluncak claimler
                    signingCredentials: signingCredentials
                );

            var handler = new JwtSecurityTokenHandler(); //tokenı oluşturcak yer bruası

            var token = handler.WriteToken(jwtSecurityToken);

            var clientTokenDto = new ClientTokenDTO
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration,
            };
            return clientTokenDto;
        }
    }
}