using AuthServer.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Repository.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<TEntity> _dbSet; //tablolarla işlem için gerekli

        public GenericRepository(AppDbContext appDbContext)
        {
            _dbContext = appDbContext;
            _dbSet = appDbContext.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbContext.Entry(entity).State = EntityState.Detached; //tracking iptal et
            }
            return entity;
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public TEntity Update(TEntity entity)
        {
            //state değiştir dön tekrar entityyi çünkü dönüş tipi entity istemişiz void olsaydı direkt dbset.update(entity) yapabiliridk.
            _dbContext.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public IQueryable<TEntity> Where(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
    }
}