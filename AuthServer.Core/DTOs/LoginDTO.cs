using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.DTOs
{
    //token oluşturmak için girişte istenilen bilgiler veritabanıyla eşleişirse bu bilgiler token döncez.
    public class LoginDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}