using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
    //token serviste kullanılcak
    //geriye dönülcek tokenDTO modeli
    public class TokenDTO
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; } //bu zorunlu değil ömrüne kolay ulaşabilmek için verdik
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; } //bu zorunlu değil ömrüne kolay ulaşabilmek için verdik
    }
}