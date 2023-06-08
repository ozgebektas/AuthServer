using AuthServer.Core.Dtos;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : CustomBaseController
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        [HttpPost]

        public async Task<IActionResult> CreateToken(LoginDto loginDto)
        {
            var result=await _authenticationService.CreateTokenAsync(loginDto);
            return ActionResultInstance(result);
        }

        [HttpPost]
        public IActionResult CreateTokenByClient(ClientLoginDto clientloginDto)
        {//bu entpoint sadece miniapi3'e firiş yapıcak. çünkü user kullanılmayan bir site gibi düşüneceğiz.
            var result=_authenticationService.CreateTokenByClient(clientloginDto);
            return ActionResultInstance(result);

        }
        [HttpPost]
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authenticationService.RevokeRefreshToken(refreshTokenDto.Token);
            return ActionResultInstance(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTokenByRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authenticationService.CreateTokenByRefreshToken(refreshTokenDto.Token);
            return ActionResultInstance(result);
        }
       
    }
}
