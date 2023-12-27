using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SharedLibrary.DTOs;
using SharedLibrary.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
    //bu handler da exceptionları kendi  oluşturduğumuz response türünde dönmek için oluşturduk.
    public static class CustomExceptionHandler
    {
        public static void UseCustomException(this IApplicationBuilder app)
        {
            //uygulamadaki tüm hataları yakalayan middleware
            app.UseExceptionHandler(configure =>
            {
                //sonlandırıcı middleware bu aşamadan sonra bir sonraki middleware gitmez istek bizz de bunu istiyoruz
                //çünkü hataları yakalıyoruz burda
                configure.Run(async context =>
                {
                    context.Response.StatusCode = 500; //server hatası geldi kendi içinde o yüzden 500
                    context.Response.ContentType = "app/json";
                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>(); //hataları al

                    if (errorFeature != null) //hata var ise
                    {
                        var exception = errorFeature.Error;

                        ErrorDTO errorDTO = null;

                        if (exception is CustomException) //eğer bu exception benim oluşturduğum customexception ise
                        {
                            errorDTO = new ErrorDTO(exception.Message, true);
                        }
                        else //custom exception değilse uygulamanın kendi içinde meydana gelmiş bir hata bu sebeple kullanıcıya gösterme false yap!!
                        {
                            errorDTO = new ErrorDTO(exception.Message, false);
                        }
                        //response nesnesi oluştur
                        var response = ResponseDTO<NoContentDTO>.Fail(StatusCodes.Status500InternalServerError, errorDTO);

                        //datayı yazmak için json serialize etmen lazım, json olarak göndermen gerekiyor yani.

                        //await context.Response.WriteAsJsonAsync(response); //alağıdaki metodla aynı işi yapıyor sanırım.

                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                });
            });
        }
    }
}