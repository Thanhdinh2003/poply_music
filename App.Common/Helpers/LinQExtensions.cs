using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace App.Common.Helpers
{
    public static class LinQExtensions
    {
        public static async Task<T> GetByIdAsync<T>(this IQueryable<T> queryable, Guid id)
            where T : class
        {
            return await queryable.FirstOrDefaultAsync(p => EF.Property<Guid>(p, "Id") == id)
                ?? throw new ArgumentException($"{nameof(T)} with id = {id} not found.");
        }

        public static async Task<T> GetByIdAsync<T>(this DbContext context, Guid id)
            where T : class
        {
            return await context.Set<T>().FirstOrDefaultAsync(p => EF.Property<Guid>(p, "Id") == id)
                ?? throw new ArgumentException($"{nameof(T)} with id = {id} not found.");
        }

        public static async Task<List<T>> GetByIdListAsync<T>(this IQueryable<T> queryable, List<Guid> idList)
            where T : class
        {
            return await queryable.Where(p => idList.Contains(EF.Property<Guid>(p, "Id"))).ToListAsync();
        }
        public static async Task<List<T>> GetByIdListAsync<T>(this DbContext context, List<Guid> idList)
           where T : class
        {
            return await context.Set<T>().Where(p => idList.Contains(EF.Property<Guid>(p, "Id"))).ToListAsync();
        }

        private static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }

        public static IQueryable<T> OrderByFieldName<T>(this IQueryable<T> queryable, string? fieldName, bool isAsc = true)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return queryable;
            }
            if (isAsc)
            {
                return queryable.OrderBy(ToLambda<T>(fieldName));
            }
            return queryable.OrderByDescending(ToLambda<T>(fieldName));
        }

        public static IQueryable<T> OrderByFieldName2<T>(this IQueryable<T> queryable, string? fieldName, bool isAsc = true)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                return queryable;
            }
            if (isAsc)
            {
                return queryable.OrderBy(p => EF.Property<Guid>(p!, fieldName));
            }
            return queryable.OrderByDescending(p => EF.Property<Guid>(p!, fieldName));
        }

        public static IQueryable<T> GetPagination<T>(this IQueryable<T> queryable, int page, int pageSize)
        {
            return queryable.Skip((page - 1) * pageSize).Take(pageSize);
        }

        public static IQueryable<T> GetPagination<T>(this IQueryable<T> queryable, int page, int pageSize, string fieldName, bool isAsc = true)
        {
            if (isAsc)
            {
                return queryable.OrderBy(ToLambda<T>(fieldName)).GetPagination(page, pageSize);
            }
            return queryable.OrderByDescending(ToLambda<T>(fieldName)).GetPagination(page, pageSize);
        }
    }
}
