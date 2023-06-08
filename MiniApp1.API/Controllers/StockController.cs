using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace MiniApp1.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetStock()
        {
            var userName=HttpContext.User.Identity.Name;//buradaki endpointe(GetStock) istek yaptığımızda isteğin tokenin payload kısmındaki name'i direkt alır
            var userIdClaim=User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            //database de ister userId ile ister userName ile stock bilgilerini eşleştirebiliriz.(gerekli dataları çekebiliriz)

            return Ok($" Stok işlemleri => UserName:{userName}-UserId:{userIdClaim.Value}");
        
        }
    }
}
