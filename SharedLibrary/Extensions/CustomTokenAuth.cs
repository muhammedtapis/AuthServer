using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configuration;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
    //jwt kodları olcak program.cs dosyasında auth metodlarını burada tanımlayıp miniapi lerde çağırcaz
    //jwt kütüphanesini ekleidk
    public static class CustomTokenAuth
    {
        //token auth. ekleme extension  metot
        public static void AddCustomTokenAuth(this IServiceCollection services, CustomTokenOptions tokenOptions)
        {
            //authentcation ekleme buapimiz hem token dağıtıyor hem de token doğrulama işlemi gerçekleştiriyor.
            services.AddAuthentication(options =>
            {
                //bu şema ne işe yarıo ,ili tane ayrı üyelik sistemin olabilir farklı login kısımları kullanıcı girişi-bayigirişi gibi bunlara şema diyoruz.
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //burası authenticationdan gelen şema
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //iki şemayı birbiriyle konuşturan bağlayan yer
                                                                                         //yani burdaki authentication JWT json web token kullanıcağını ve aşağıdaki jwtScheme kullanıcağını bilsin
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
            {
                //burası bearerdan gelen şema,yukarıdaki şemayla konuşturmamız lazım

                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    //burası token geçerlilik adımları gibi düşün gelen token ile appsettingsteki alanlar karşılaştırılcak o belirtiliyor.
                    //burada da validate edilcek alanları veriyoruz
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience[0],
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    //validate yani kontrol kısmı appsettingsteki alanlarla aynı ım onu kontrol et
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,

                    //default 5 dk verir iki api serveri farklı zonelarda çalıştırabilirsin zaman farkı olabilir o sebeple bunu yapıo
                    //biz sıfıra çektik zaman farkımız yok
                    ClockSkew = TimeSpan.Zero
                };
            });
        }
    }
}