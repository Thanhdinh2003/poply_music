using App.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq.Expressions;

namespace App.Common.Base
{
    public abstract class BaseRepository<T, TContext>
        where TContext : DbContext
        where T : class
    {
        protected readonly TContext _context;
        protected readonly ILogger<BaseRepository<T, TContext>> _logger;
        public BaseRepository(TContext context, ILogger<BaseRepository<T, TContext>> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            IQueryable<T> query = _context.Set<T>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return await query.FirstOrDefaultAsync()

                ?? throw new Exception();
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(p => EF.Property<Guid>(p, "Id") == id)
                ?? throw new ArgumentException($"{nameof(T)} with id = {id} not found.");
        }

        public virtual async Task<List<T>> GetByIdListAsync(List<Guid> idList)
        {
            return await _context.Set<T>().Where(p => idList.Contains(EF.Property<Guid>(p, "Id"))).ToListAsync();
        }

        public virtual async Task<int> DeleteByIdAsync(Guid id)
        {
            try
            {
                var item = await _context.Set<T>().FirstOrDefaultAsync(p => EF.Property<Guid>(p, "Id") == id)
               ?? throw new ArgumentException($"{nameof(T)} with id = {id} not found.");
                _context.Set<T>().Remove(item);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed while deleting {typeof(T)} with id = {id}, message = {ex.Message}");
            }
        }

        public virtual async Task<int> DeleteByIdListAsync(List<Guid> idList)
        {
            try
            {
                var itemList = await _context.Set<T>().Where(p => idList.Contains(EF.Property<Guid>(p, "Id"))).ToListAsync();
                _context.Set<T>().RemoveRange(itemList);
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed while deleting {nameof(T)}, message = {ex.Message}");
            }
        }

        public virtual async Task<T> InsertOne(T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed while inserting new {nameof(T)}: {JsonConvert.SerializeObject(entity)}, message = {ex.Message}");
            }
        }

        public virtual async Task<List<T>> InsertMany(List<T> entities)
        {
            try
            {
                await _context.Set<T>().AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                return entities;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed while inserting new list of {nameof(T)}, message = {ex.Message}");
            }
        }

        public virtual async Task<BasePagination<T>> SearchWithPagination(Expression<Func<T, bool>> predicate, string? orderBy, bool isAsc = true, int page = 1, int pageSize = 10)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().AsQueryable();

                if (predicate != null)
                {
                    query = query.Where(predicate);
                }

                query = query.Distinct();

                var totalItems = query.Count();

                if (!string.IsNullOrWhiteSpace(orderBy))
                {
                    query = query.OrderByFieldName(orderBy, isAsc);
                }

                var items = await query.GetPagination(page, pageSize).ToListAsync();

                return new BasePagination<T>(totalItems, page, pageSize, items);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }
}
