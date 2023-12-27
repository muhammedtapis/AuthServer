using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
    //fluent validation görmek için program cs extension
    //kendi oluşturdüumuz responseDTo sınıfını dönmek için oluşturduk!!!
    public static class CustomValidationResponse
    {
        public static void UseCustomValidationResponse(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                //modelstate invalid oldğunda response oluştur.
                options.InvalidModelStateResponseFactory = context =>
                {
                    // hata sayısı 0 dan büyükse yani hata varsa,hataları seç,sonra hataların mesajını seç ve errors ata
                    var errors = context.ModelState.Values.Where(x => x.Errors.Count() > 0).SelectMany(x => x.Errors).Select(x => x.ErrorMessage);

                    ErrorDTO errorDTO = new ErrorDTO(errors.ToList(), true); //errordto ata bu hataları

                    var response = ResponseDTO<NoContentDTO>.Fail(StatusCodes.Status400BadRequest, errorDTO); //response oluştur

                    return new BadRequestObjectResult(response);
                };
            });
        }
    }
}