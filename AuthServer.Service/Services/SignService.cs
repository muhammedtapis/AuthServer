using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    //bu sınıf appsettingsten gelen security keyi simetrik key olarka dönecek onun için oluşturduk tokenserviste kullancaz.
    public static class SignService
    {
        public static SecurityKey GetSymmetricSecurityKey(string key)
        {
            //aldığı stringi bytelarını alıp utf8 encoding çözüp Simetrik security key olarak döner
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }
    }
}