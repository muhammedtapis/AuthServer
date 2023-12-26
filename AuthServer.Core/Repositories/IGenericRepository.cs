using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Repositories
{
    //generic crud işlemleri ,delete,update,create
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);

        Task<IEnumerable<TEntity>> GetAllAsync();

        Task AddAsync(TEntity entity);

        Task<TEntity> Update(TEntity entity);

        void Remove(TEntity entity);

        //func delege temsil ediyor metodları temsıl eder delegeler. where(x => x.Id > 5) x Tentity x.Id>5 bool sonuç
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate); //LINQ expression func alcak bu fun  TEntity alıp bool döncek
    }
}