using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IServiceGeneric<TEntity,TDto> where TEntity : class where TDto : class
    {
        Task<Response<TDto>> GetByIdAsync(int id);
        Task<Response<IEnumerable<TDto>>> GetAllAsync();

        Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate);

        Task<Response<TDto>> AddAsync(TDto entity);
        Task<Response<NoDataDto>> Remove(int id);//burada birşey dönmeyeceği için ve biizm ürettiğimiz response model hepsi bir mesaj ve bool gibi değerler döndürüyor
        //eğer burada bir şey döndermeyeceksek bir class oluşturup(Nodatadto) bu class ı döndermiş gibi gösteririz.

        Task<Response<NoDataDto>> Update(TDto entity,int id);
    }
}
