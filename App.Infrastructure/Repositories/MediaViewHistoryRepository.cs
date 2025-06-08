using App.Application.Contracts.Infrastructure;
using App.Common.Base;
using App.Common.Helpers;
using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.Infrastructure.Repositories
{
    public class MediaViewHistoryRepository : BaseRepository<MediaViewHistory, DatabaseContext>, IMediaViewHistoryRepository
    {
        private readonly ILogger<MediaViewHistoryRepository> _logger;
        private readonly DatabaseContext _context;
        public MediaViewHistoryRepository(DatabaseContext context, ILogger<MediaViewHistoryRepository> logger)
            : base(context, logger)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<BasePagination<MediaViewHistory>> Search(Guid userId, Guid mediaId, Guid categoryId, Guid authorId, string orderBy = "name", bool isAsc = true, int page = 1, int pageSize = 8)
        {
            try
            {
                var query = _context.MediaViewHistory.Include(p => p.Media).AsQueryable();

                if(userId != Guid.Empty)
                {
                    query = query.Where(p => p.UserId == userId);
                }

                if (categoryId != Guid.Empty)
                {
                    query = query.Where(p => p.Media != null && p.Media.CategoryId == categoryId);
                }

                if (authorId != Guid.Empty)
                {
                    query = query.Where(p => p.Media != null && p.Media.AuthorId == authorId);
                }

                if (mediaId != Guid.Empty)
                {
                    query = query.Where(p => p.MediaId == mediaId);
                }

                var totalItems = query.Count();
                var items = await query.GetPagination(page, pageSize, orderBy, isAsc).ToListAsync();

                return new BasePagination<MediaViewHistory>(totalItems, page, pageSize, items);
            }
            catch(Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }
    }
}
