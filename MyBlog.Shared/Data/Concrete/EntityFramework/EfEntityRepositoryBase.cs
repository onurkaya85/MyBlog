using LinqKit;
using Microsoft.EntityFrameworkCore;
using MyBlog.Shared.Data.Abstract;
using MyBlog.Shared.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Shared.Data.Concrete.EntityFramework
{
    public class EfEntityRepositoryBase<T> : IRepository<T> where T : class, IBaseEntity, new()
    {
        protected readonly DbContext _context;
        internal DbSet<T> _dbSet;

        public EfEntityRepositoryBase(DbContext context)
        {
            _context = context;
            this._dbSet = _context.Set<T>();
            //_context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;  //AsNoTracking'in yaptığı işi yapar.ister bunu kullan ister AsNoTracking'i
        }

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            await Task.Run(() => { _dbSet.Update(entity); });
            return entity;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await (predicate == null ? _dbSet.AnyAsync() : _dbSet.AnyAsync(predicate));
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await (predicate == null ? _dbSet.CountAsync() :_dbSet.CountAsync(predicate));
        }

        public async Task DeleteAsync(T entity)
        {
            await Task.Run(() => { _dbSet.Remove(entity); });
        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (includeProperties.Any())
            {
                foreach (var item in includeProperties)
                    query = query.Include(item);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;
            query = query.Where(predicate);

            if (includeProperties.Any())
            {
                foreach (var item in includeProperties)
                    query = query.Include(item);
            }
            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IList<T>> SearchAsync(IList<Expression<Func<T, bool>>> predicates, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet;
            if(predicates.Any())
            {
                var predicateChain = PredicateBuilder.New<T>();
                foreach (var predicate in predicates)
                {
                    //query = query.Where(predicate);
                    predicateChain.Or(predicate);
                }
                query = query.Where(predicateChain);
            }
            if(includeProperties.Any())
            {
                foreach (var item in includeProperties)
                    query = query.Include(item);
            }
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<T> GetAsyncV2(IList<Expression<Func<T, bool>>> predicates, IList<Expression<Func<T, object>>> includeProperties)
        {
            IQueryable<T> query = _dbSet;
            if(predicates != null && predicates.Any())
            {
                foreach(var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
            }
            if (includeProperties != null && includeProperties.Any())
            {
                foreach (var item in includeProperties)
                    query = query.Include(item);
            }
            return await query.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IList<T>> GetAllAsyncV2(IList<Expression<Func<T, bool>>> predicates, IList<Expression<Func<T, object>>> includeProperties)
        {
            IQueryable<T> query = _dbSet;
            if (predicates != null && predicates.Any())
            {
                foreach (var predicate in predicates)
                {
                    query = query.Where(predicate);
                }
            }
            if (includeProperties != null && includeProperties.Any())
            {
                foreach (var item in includeProperties)
                    query = query.Include(item);
            }
            return await query.AsNoTracking().ToListAsync();
        }
    }
}
