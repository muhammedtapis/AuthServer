using AuthServer.Core.DTOs;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    //controllerda çağıracağımız servis token işlemleri burada yapılacak ITokenService interface metodlarını kullancağımız yer
    //kimlik doğrulama işlemlerini yapacağımız yer
    public interface IAuthenticationService
    {
        Task<ResponseDTO<TokenDTO>> CreateTokenAsync(LoginDTO loginDto);

        Task<ResponseDTO<TokenDTO>> CreateTokenByRefreshTokenAsync(string refreshToken);

        //refreshtoken sonlandırması gerekli çünkü refresh token sahip client sürekli access token alabilir
        Task<ResponseDTO<NoContentDTO>> RevokeRefreshTokenAsync(string refreshToken);

        //clientId ve secret ile oluşturulacak token metot
        Task<ResponseDTO<ClientTokenDTO>> CreateTokenByClientAsync(ClientLoginDTO clientLoginDto);
    }
}