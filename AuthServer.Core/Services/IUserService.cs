using AuthServer.Core.DTOs;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    //veritabanına kullanıcı bilgileir kaydetme ve gösterme servisi
    //burada aslında veritabanı ile ilgili işlemler var ama repositorysini oluşturmaduk çünkü Identity kütüphanesi kullandığımız için
    //hazır metotlar geliyor UserManager üzerinden!!
    public interface IUserService
    {
        Task<ResponseDTO<UserAppDTO>> CreateUserAsync(CreateUserDTO createUserDTO);

        Task<ResponseDTO<UserAppDTO>> GetUserByName(string userName);
    }
}