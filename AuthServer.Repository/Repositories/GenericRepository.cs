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
            //tracking sorunundna dolayı aşağıdaki kodu iptal ettik update metodunda getbyid çağırdığımızda efcore track ediyor
            //objectmapperile getbyid metodundan gelen entity ile kullanıcıdan gelen dto mapleniyor böylelikle efcore kendisi state değiştiriyor
            //update metodu çağırmadan mapleyerek sadece değiştirilen yerlerin üzerine yazıyor efcore.

            //if (entity != null)
            //{
            //    //_dbContext.Entry(entity).State = EntityState.Detached; //tracking iptal et
            //}
            return entity;
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        //bu metod task dönmeyince threadi blokluyor servis metodunda iki tane repository metodu çağırıyoruz
        //getbyid ve update ikisi de aynı dbcontext kullandığı için server error alıyoruz
        public Task<TEntity> Update(TEntity entity)
        {
            //state değiştir dön tekrar entityyi çünkü dönüş tipi entity istemişiz void olsaydı direkt dbset.update(entity) yapabiliridk.
            _dbContext.Entry(entity).State = EntityState.Modified;
            //_dbSet.Update(entity);
            return Task.FromResult(entity);
        }

        public IQueryable<TEntity> Where(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
    }
}