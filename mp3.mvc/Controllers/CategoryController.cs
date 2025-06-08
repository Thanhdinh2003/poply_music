using App.Application.Contracts.Infrastructure;
using App.Common.Base;
using App.Domain.Entities;
using App.Infrastructure;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mp3.mvc.Models;

namespace mp3.mvc.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly INotyfService _notyfService;
        private readonly IMediaRepository _mediaRepository;
        private readonly DatabaseContext _databaseContext;

        public CategoryController(ILogger<CategoryController> logger, INotyfService notyfService, IMediaRepository mediaRepository,
            DatabaseContext databaseContext)
        {
            _logger = logger;
            _notyfService = notyfService;
            _mediaRepository = mediaRepository;
            _databaseContext = databaseContext;
        }
        public async Task<IActionResult> Index(string searchText = "", int page = 1, int pageSize = 10)
        {
            var query = _databaseContext.Categories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p => !string.IsNullOrWhiteSpace(p.Name) && p.Name.ToLower().Contains(searchText.Trim().ToLower()));
            }

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new CategorySearchItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                })
                .ToListAsync();

            foreach (var item in items)
            {
                var numberOfTrack = await _databaseContext.Media.Where(p => p.CategoryId == item.Id).CountAsync();
                item.NumberOfTracks = numberOfTrack;
            }

            ViewData["searchText"] = searchText;
            ViewBag.Data = new BasePagination<CategorySearchItemViewModel>(total, page, pageSize, items);
            return View();
        }

        public async Task<IActionResult> GetDetail(Guid id)
        {
            var category = await _databaseContext.Categories.Where(p => p.Id == id).AsNoTracking().FirstOrDefaultAsync();


            ViewBag.Data = category;


            var tracks = await _databaseContext.Media
                .Include(p => p.MediaContent)
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Where(p => p.CategoryId == id)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
            ViewBag.Tracks = tracks;

            var views = await _databaseContext.Media
                .Include(p => p.MediaViewHistory)
                .Where(p => p.CategoryId == id)
                .OrderByDescending(p => p.CreatedAt)
                .SumAsync(p => p.MediaViewHistory.Count);

            ViewData["Views"] = views;

            return View();
        }

        public async Task<IActionResult> Update(Guid id)
        {
            var category = await _databaseContext.Categories.Where(p => p.Id == id).AsNoTracking().FirstOrDefaultAsync();

            if (category == null)
            {
                _notyfService.Error("Không tìm thấy thể loại", 2);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Category category)
        {
            var categoryEntity = await _databaseContext.Categories.Where(p => p.Id == category.Id).FirstOrDefaultAsync();
            categoryEntity.Name = category.Name;
            var changeCount = await _databaseContext.SaveChangesAsync();

            if (changeCount > 0)
            {
                _notyfService.Success("Cập nhật thành công", 2);
                return RedirectToAction(nameof(GetDetail), new { id = category.Id });
            }
            _notyfService.Error("Cập nhật thất bại", 2);
            return RedirectToAction(nameof(Update), new { id = category.Id });
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var author = await _databaseContext.Categories.Where(p => p.Id == id).AsNoTracking().FirstOrDefaultAsync();

            return View(author);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmation(Category request)
        {
            var category = await _databaseContext.Categories.Where(p => p.Id == request.Id).FirstOrDefaultAsync();

            _databaseContext.Remove(category);

            var changeCount = await _databaseContext.SaveChangesAsync();

            if (changeCount > 0)
            {
                _notyfService.Success("Xóa thành công", 2);
                return RedirectToAction(nameof(Index));
            }
            _notyfService.Error("Xóa thất bại", 2);
            return RedirectToAction(nameof(Delete), new { id = category.Id });
        }

        public async Task<IActionResult> Add()
        {
            var author = new Category();
            author.Name = "";

            return View(author);
        }


        [HttpPost]
        public async Task<IActionResult> Add(Category request)
        {

            await _databaseContext.AddAsync(request);

            var changeCount = await _databaseContext.SaveChangesAsync();

            if (changeCount > 0)
            {
                _notyfService.Success("Thêm thành công", 2);
                return RedirectToAction(nameof(Index));
            }
            _notyfService.Error("Thêm thất bại", 2);
            return RedirectToAction(nameof(GetDetail), new { id = request.Id });
        }
    }
}
