using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using SharedLibrary.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOption _tokenOption;
        public TokenService(UserManager<UserApp> userManager,IOptions<CustomTokenOption> options)
        {
            _userManager = userManager;
            _tokenOption = options.Value;
        }

        private string CreateRefreshToken()
        {
            var numberByte=new byte[32];//32 byte tipinde bir byte tipi üretildi:numberbyte
            using var rnd=RandomNumberGenerator.Create();//random bir değer üretildi.
            rnd.GetBytes(numberByte);//üretilen bu random değeri bytlerina al ve bunu numberbyte aktar.
            
            return Convert.ToBase64String(numberByte);//byte string değer döndürecek.
        }
        private IEnumerable<Claim> GetClaims(UserApp userApp,List<String> auidence)
        {//audiences tokenin hangi apilere istek yapacağına karşılık gelir.

            var userList = new List<Claim>//kullanıcı ile hangi claimler eklenecek bunları eklediğimiz kısım.(token için payload kısmı)
            {
                new Claim(ClaimTypes.NameIdentifier,userApp.Id),
                new Claim(JwtRegisteredClaimNames.Email,userApp.Email),
                new Claim(ClaimTypes.Name,userApp.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())

            };
            //AddRange method is used to add the objects/elements of a collection at the end of the list.
            userList.AddRange(auidence.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            return userList;
            //userlist= token ürettikten sonra tokenin payload kısmında bulunacak yer

        }

        //üyelik sistemi gerekmeyen durumlarda bu method kullanılacak.
        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            var claims = new List<Claim>();
            claims.AddRange(client.Audiences.Select(x=>new Claim(JwtRegisteredClaimNames.Aud,x)));

            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString());
            new Claim(JwtRegisteredClaimNames.Sub,client.Id.ToString());

            return claims;
        }

        public TokenDto CreateToken(UserApp userApp)
        {
            /*create token metodu client authservera request gönderdi authserver db ye bakıp kullanıcı olup olmadığına baktı 
             * eğer varsa response döndü.burada respponse olarak token dönecek.yani bir token üretilecek*/

            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
            var refreshTokenExpiration=DateTime.Now.AddMinutes(_tokenOption.RefreshTokenExpiration);
            var securityKey=SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);//simetrik tipte oluşan securityKey algoritma yapısında düzenliyoruz.
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaims(userApp, _tokenOption.Audience),
                signingCredentials: signingCredentials);

            var handler = new JwtSecurityTokenHandler();//bu bir token oluşturacak.
            var token = handler.WriteToken(jwtSecurityToken);
            var tokenDto = new TokenDto
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration,

            };
            return tokenDto;
         
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOption.AccessTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_tokenOption.SecurityKey);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);//simetrik tipte oluşan securityKey algoritma yapısında düzenliyoruz.
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaimsByClient(client),
                signingCredentials: signingCredentials);

            var handler = new JwtSecurityTokenHandler();//bu bir token oluşturacak.
            var token = handler.WriteToken(jwtSecurityToken);
            var clienttokenDto = new ClientTokenDto
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration,
             
            };
            return clienttokenDto;
        }
    }
}
