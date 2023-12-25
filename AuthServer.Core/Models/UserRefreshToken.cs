using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Models
{
    public class UserRefreshToken
    {
        public string UserId { get; set; } //refresh token kime ait olaca ?
        public string Code { get; set; } //token kendisi kodu yani
        public DateTime Expiration { get; set; } //token süresi
    }
}