using AuthServer.Core.Dtos;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Exceptions;
using System.Threading.Tasks;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : CustomBaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
           _userService = userService;
        }
        [HttpPost]
        //bu endpoint user oluşşruruken token oluşturacağı için authorize işemine gerek yok.
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            throw new CustomException("bir hata meydana geldi");
            return ActionResultInstance(await _userService.CreateUserAsync(createUserDto));
        }

        [Authorize]//bu endpoint mutlaka bir token istiyor.
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            //burada HttpContext.User.Identity.Name direkt olarak eşleyecek. çünkü;Token Service de ClaimTypes.Name,userApp.UserName belirttik
            //token isteği üzerinden name'i bulur.(Token içerisinden name claim i buluyor.)
            return ActionResultInstance(await _userService.GetUserByNameAsync(HttpContext.User.Identity.Name)); 
        }

        
    }
}
