using App.Application.Contracts.Infrastructure;
using App.Common.Base;
using App.Common.Helpers;
using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace App.Infrastructure.Repositories
{
    public class MediaRepository : BaseRepository<Media, DatabaseContext>, IMediaRepository
    {
        private readonly ILogger<MediaRepository> _logger;
        private readonly DatabaseContext _context;
        public MediaRepository(DatabaseContext context, ILogger<MediaRepository> logger) : base(context, logger)
        {
            _context = context;
            _logger = logger;
        }
        public override async Task<Media> GetByIdAsync(Guid id)
        {
            return await _context.Media.Include(p => p.MediaContent)
                .Include(p => p.Author)
                .Include(p => p.Category).FirstOrDefaultAsync(p => EF.Property<Guid>(p, "Id") == id)
                ?? throw new ArgumentException($"Cannot find media with id = {id}");
        }

        public async Task<BasePagination<Media>> GetAvailableItemList(Guid userId, string orderBy = "name", bool isAsc = true, int page = 1, int pageSize = 8)
        {
            try
            {
                var query = (from m in _context.Media
                             select m).Distinct();

                var totalItems = query.Count();
                var items = await query.Include(p => p.MediaContent).GetPagination(page, pageSize, orderBy, isAsc).ToListAsync();
                return new BasePagination<Media>(totalItems, page, pageSize, items);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BasePagination<Media>> GetFavouriteItemList(Guid userId, string orderBy = "name", bool isAsc = true, int page = 1, int pageSize = 8)
        {
            try
            {
                var query = (from m in _context.Media
                             select m).Distinct();

                var totalItems = query.Count();
                var items = await query.Include(p => p.MediaContent).GetPagination(page, pageSize).ToListAsync();
                return new BasePagination<Media>(totalItems, page, pageSize, items);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<BasePagination<Media>> GetPurchasedItemList(Guid userId, string orderBy = "name", bool isAsc = true, int page = 1, int pageSize = 8)
        {
            try
            {
                var query = _context.Media;

                var totalItems = query.Count();
                var items = await query.Include(p => p.MediaContent).Include(p => p.Author).GetPagination(page, pageSize).ToListAsync();
                return new BasePagination<Media>(totalItems, page, pageSize, items);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Media>> GetTrendingItemList(bool isAsc = true, int page = 1, int pageSize = 8)
        {
            try
            {
                /*var query = _context.MediaViewHistory
                    .GroupBy(p => p.MediaId)
                    .OrderByDescending(p => p.Count())
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(g => _context.Media.Include(p => p.MediaContent).Include(p => p.Author).FirstOrDefault(m => m.Id == g.Key));*/

                var query = _context.Media
                    .Include(p => p.MediaContent)
                    .Include(p => p.Author)
                    .Include(p => p.Category)
                    .Include(p => p.MediaViewHistory)
                    .OrderByDescending(p => p.MediaViewHistory.Count);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Media>> GetSuggestedItemList(Guid userId, int pageSize = 8)
        {
            // lấy danh sách category và author mà user đã xem nhiều nhất
            var topAuthors = await _context.MediaViewHistory
                .Where(h => h.UserId == userId)
                .GroupBy(h => h.Media.AuthorId)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToListAsync();

            var topCategories = await _context.MediaViewHistory
                .Where(h => h.UserId == userId)
                .GroupBy(h => h.Media.CategoryId)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToListAsync();

            // đề xuất các media mới theo top author hoặc category (chưa xem)
            var recommended = await _context.Media
                .Where(m => (topAuthors.Contains(m.AuthorId) || topCategories.Contains(m.CategoryId))
                    && !m.IsHidden && !m.IsLocked)
                //.OrderBy(m => Guid.NewGuid()) // chậm với nhiều dữ liệu
                .OrderBy(x => EF.Functions.Random())
                .Take(10)
                .ToListAsync();

            return recommended;
        }
    }
}
