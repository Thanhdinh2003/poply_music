using App.Application.Contracts.Infrastructure;
using App.Common.Base;
using App.Common.Helpers;
using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.Infrastructure.Repositories
{
    public class AuthorRepository : BaseRepository<Author, DatabaseContext>, IAuthorRepository
    {
        private readonly ILogger<AuthorRepository> _logger;
        private readonly DatabaseContext _context;
        public AuthorRepository(DatabaseContext context, ILogger<AuthorRepository> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BasePagination<Author>> SearchAuthor(Guid categoryId, string? name, string orderBy = "name", bool isAsc = false, int page = 1, int pageSize = 10)
        {
            try
            {
                IQueryable<Author> query = from a in _context.Authors
                                           join m in _context.Media
                                           on a.Id equals m.AuthorId
                                           where (string.IsNullOrWhiteSpace(name) || a.Name!.ToLower().Contains(name.ToLower()) || m.Name!.ToLower().Contains(name.Trim().ToLower()))
                                           && (categoryId == Guid.Empty || m.CategoryId == categoryId)
                                           select a;
                query = query.Distinct();
                var totalItems = query.Count();
                var items = await query.GetPagination(page, pageSize, orderBy, isAsc).ToListAsync();
                return new BasePagination<Author>(totalItems, page, pageSize, items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new ArgumentException(ex.Message);
            }
        }
    }
}
