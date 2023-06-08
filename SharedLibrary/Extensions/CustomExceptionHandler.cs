using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Dtos;
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
    public static class CustomExceptionHandler
    {
        public static void UseCustomException(this IApplicationBuilder app)
        {
            //UseExceptionHandler middleware'i bize uygulama içerisindeki tüm hatalrı yakalamamızı sağlayacak.
            app.UseExceptionHandler(config =>
            {
                //run sonlandırıcı middleware bir sonraki middleware geçiş olmaz run middleware'ni kullandıktan sonra
                config.Run(async context =>
                {
                    context.Response.StatusCode = 500;//kendi içerisinde bir hata olduğunu(yani client da bir hata olmadığını belirtmek için)
                    /*ayrıca burada oluşturduğumuz UseCustomExceptin middleware'ni biz startup da en başta belirteceğiz ki diğer middleware lara girmeden buradaki hatayı bzie versin*/
                    context.Response.ContentType = "application/json";
                    //IExceptionHandlerFeature interface üzerinden hatalarımızı yakalayacağız.
                    var errorFeature =context.Features.Get<IExceptionHandlerFeature>();

                    if(errorFeature != null )
                    {
                        var ex = errorFeature.Error;
                        ErrorDto errorDto = null;

                        if(ex is CustomException)
                        {
                            errorDto = new ErrorDto(ex.Message,true);
                        }
                        else
                        {
                            //uygulamanın kendi içerisindeki hatası olduğunu belirttik
                            errorDto = new ErrorDto(ex.Message, false);
                        }
                        var response = Response<NoDataDto>.Fail(errorDto, 500);
                        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                    }
                   
                });
            });
        }
    }
}
