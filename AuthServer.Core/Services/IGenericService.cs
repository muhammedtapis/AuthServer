using SharedLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    //direkt Dto nesnesi dönen servs metodları
    public interface IGenericService<TEntity, TDto> where TEntity : class where TDto : class
    {
        Task<ResponseDTO<TDto>> GetByIdAsync(int id);

        Task<ResponseDTO<IEnumerable<TDto>>> GetAllAsync();

        Task<ResponseDTO<TDto>> AddAsync(TDto dto);

        Task<ResponseDTO<NoContentDTO>> UpdateAsync(TDto dto, int id);

        Task<ResponseDTO<NoContentDTO>> RemoveAsync(int id);

        Task<ResponseDTO<IEnumerable<TDto>>> WhereAsync(Expression<Func<TEntity, bool>> predicate);
    }
}