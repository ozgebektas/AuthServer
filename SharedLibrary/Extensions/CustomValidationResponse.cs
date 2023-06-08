﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
    public static class CustomValidationResponse
    {
        public static void UseCustomValidationResponse(this IServiceCollection services)
        {
            //Api davranışının üzerinde ayarlamalar yapacağız.
            services.Configure<ApiBehaviorOptions>(options =>
            {//InvalidModelStateResponseFactory=modelstate invalid olduğunda yani;
                //örneğin propertyler client a eksik gönderildiğinde çalışacak yapı
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.Values.Where(x => x.Errors.Count > 0).SelectMany(x => x.Errors).Select(x => x.ErrorMessage);
                    ErrorDto errorDto = new ErrorDto(errors.ToList(), true);
                    var response = Response<NoContentResult>.Fail(errorDto, 400);

                    return new BadRequestObjectResult(response);
                };
            });
        }
    }
}