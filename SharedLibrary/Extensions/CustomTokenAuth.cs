using SharedLibrary.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{//Extensions methodları static method olarak tanımlanır.
    public static class CustomTokenAuth
    {
        //start-up da bulunan IserviceCollection için bir extension yapısı burada kurulacak.
        //çünkü burada mini api için startuplarını configure etmemiz gerekli

        public static void AddCustomTokenAuth(this IServiceCollection services,CustomTokenOption tokenOptions) 
        {
            
            services.AddAuthentication(options =>
            {
                
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
               
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    //neylerle doğrulayacağımız kısım
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience[0],//audience de ilk index ıssuerin kendisi
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    //alttaki değerler nelerin doğrulanacağını bildirdiğimiz kısım(kontrol etmek istediğimiz kısımlar)
                    ValidateIssuerSigningKey = true,//üretilecek token değerinin uygulamamıza ait bir değer olduğunu ifade eden security key verisinin doğrulanmasıdır.
                    ValidateAudience = true,//oluşturulacak token değerlerini kimlerin hangi origin/sitelerin kullanacağını belirlediğimiz kısım.
                    ValidateIssuer = true,//oluşturulacak token değerini kimin dağıttığını ifade edeceğimiz alandır.
                    ValidateLifetime = true,//oluşturulan token değerinin süresini kontrol edecek olan doğrulamadır.
                    ClockSkew = TimeSpan.Zero,

                    //ValidIssuer = Configuration["TokenOptions:Issuer"]---- >>>> yapısı halinde de kullanabilirdik(tokenOptions nesnesi üretmeseydik)


                };

            });
        }
    }
}
