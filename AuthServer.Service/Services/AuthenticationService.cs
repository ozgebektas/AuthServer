using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

        public AuthenticationService(IOptions<List<Client>> optionsClient,ITokenService tokenService,
            UserManager<UserApp> userManager,IUnitOfWork unitOfWork,IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            _clients = optionsClient.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }
        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if(loginDto==null) throw new ArgumentNullException(nameof(loginDto));

            var user=await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Response<TokenDto>.Fail("Email or Password is wrong",400,true);

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Response<TokenDto>.Fail("Email or Passwor is wrong", 400, true);
            }
            var token=_tokenService.CreateToken(user);

            var userRefreshToken=await _userRefreshTokenService.Where(x=>x.UserId==user.Id).SingleOrDefaultAsync();
            
            if (userRefreshToken == null)
            {
                await _userRefreshTokenService.AddAsync(new UserRefreshToken { UserId = user.Id, Code = token.RefreshToken, Expiration = token.RefreshTokenExpiration });
            }
            else
            {//güncelleme işlemi olacak.(refresh token üzerinde)
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;

            }
            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(token,200);
        }
        
        public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientloginDto)
        {
            var client = _clients.SingleOrDefault(x => x.Id == clientloginDto.ClientId && x.Secret == clientloginDto.ClientSecret);
            if(client == null)
            {
                return Response<ClientTokenDto>.Fail("ClientId or ClientSecret not found", 404, true);
            }

            var token=_tokenService.CreateTokenByClient(client);
            return Response<ClientTokenDto>.Success(token, 200);
        }

        public async Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            var existrefreshToken= await _userRefreshTokenService.Where(x=>x.Code == refreshToken).SingleOrDefaultAsync();
            if(existrefreshToken == null)
            {
                return Response<TokenDto>.Fail("RefreshToken not found", 404, true);
            }
            var user = await _userManager.FindByIdAsync(existrefreshToken.UserId);
            if(user == null)
            {
                return Response<TokenDto>.Fail("User not found", 404, true);

            }

            var token=_tokenService.CreateToken(user);
            existrefreshToken.Code = token.RefreshToken;
            existrefreshToken.Expiration = token.RefreshTokenExpiration;
            await _unitOfWork.CommitAsync();

            return Response<TokenDto>.Success(token, 200);
        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            //RevokeRefreshtoken veritabanına null yapılcak. log out olduğunda veritabanındaki refresh token null olacak.
            var existRefreshToken=await _userRefreshTokenService.Where(x=>x.Code==refreshToken).SingleOrDefaultAsync();
            if(existRefreshToken == null)
            {
                return Response<NoDataDto>.Fail("Refresh token not found", 404, true);

            }

            _userRefreshTokenService.Remove(existRefreshToken);

            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(200);

        }
    }
}
