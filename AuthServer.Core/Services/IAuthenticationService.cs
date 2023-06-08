using AuthServer.Core.Dtos;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    //login -kimlik doğrulama işlemini gerçekleştirecek
    //loginDto ve ClientLoginDto kulllanılır
    public interface IAuthenticationService
    {

        //hiç jwt ve refresh token üretmediğimizde login olunca yada süresi geçince üretilecek token methodu
        Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto);

        //CreateTokenByRefreshToken elimizdeki refreshtoken ile yeni bir jwt üretmek için method
        Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken);

        //log-out yaptığımızda refresh tokeni null'a set etmek için
        Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken);

        Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientloginDto);

    }
}
