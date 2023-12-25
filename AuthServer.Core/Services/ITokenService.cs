using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    //burada response dönmeyeceğiz bu metodları authorization serviste çağıracağımız için tokenDTO ve clientTokenDTO döncez
    public interface ITokenService
    {
        TokenDTO CreateToken(UserApp user); //üyelik sistemine ait kullanıcılar için üretilcek token

        //buradaki client sınıfı ne bir entity ne de bir dto appsettingten gelen bilgilerle doldurulacağı için configuration klasöründe oluşturduk.
        ClientTokenDTO CreateTokenByClient(Client client);
    }
}