using App.Application.Contracts.Infrastructure;
using App.Common.Base;
using App.Domain.Entities;
using App.Infrastructure;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mp3.mvc.Consts;
using mp3.mvc.Models;

namespace mp3.mvc.Controllers
{
    [Authorize]
    public class AuthorController : Controller
    {
        private readonly ILogger<AuthorController> _logger;
        private readonly INotyfService _notyfService;
        private readonly IMediaRepository _mediaRepository;
        private readonly DatabaseContext _databaseContext;

        public AuthorController(ILogger<AuthorController> logger, INotyfService notyfService, IMediaRepository mediaRepository,
            DatabaseContext databaseContext)
        {
            _logger = logger;
            _notyfService = notyfService;
            _mediaRepository = mediaRepository;
            _databaseContext = databaseContext;
        }

        public async Task<IActionResult> Index(string searchText = "", int page = 1, int pageSize = 10)
        {
            var query = _databaseContext.Authors
                .Where(p => p.Id != ResourceConst.AnonymousAuthor)
                .OrderBy(p => p.Name)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p => !string.IsNullOrWhiteSpace(p.Name) && p.Name.ToLower().Contains(searchText.Trim().ToLower()));
            }


            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new AuthorSearchItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    AvatarUrl = !string.IsNullOrWhiteSpace(p.AvatarUrl) ? p.AvatarUrl : ResourceConst.AuthorAvatarDefaultUrl
                })
                .ToListAsync();

            foreach (var item in items)
            {
                var views = await _databaseContext.MediaViewHistory
                    .Include(p => p.Media)
                    .Where(p => p.Media != null && p.Media.AuthorId == item.Id)
                    .CountAsync();

                var numberOfTrack = await _databaseContext.Media.Where(p => p.AuthorId == item.Id).CountAsync();

                item.Views = views;
                item.NumberOfTracks = numberOfTrack;
            }

            ViewData["searchAuthor"] = searchText;
            ViewBag.AuthorData = new BasePagination<AuthorSearchItemViewModel>(total, page, pageSize, items);
            return View();
        }

        public async Task<IActionResult> GetDetail(Guid id)
        {
            var author = await _databaseContext.Authors.Where(p => p.Id != ResourceConst.AnonymousAuthor).Where(p => p.Id == id).AsNoTracking().FirstOrDefaultAsync();

            if (author == null)
            {
                _notyfService.Error("Không tìm thấy ca sĩ", 2);
                return RedirectToAction(nameof(Index));
            }

            var views = await _databaseContext.MediaViewHistory
                     .Include(p => p.Media)
                     .Where(p => p.Media != null && p.Media.AuthorId == id)
                 .CountAsync();

            if (string.IsNullOrWhiteSpace(author.AvatarUrl))
            {
                author.AvatarUrl = ResourceConst.AuthorAvatarDefaultUrl;
            }

            ViewBag.Author = author;
            
            var tracks = await _databaseContext.Media
                .Include(p => p.MediaContent)
                .Include(p => p.Category)
                .Where(p => p.AuthorId == id)
                .OrderByDescending(p => p.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
            ViewBag.Tracks = tracks;
            ViewData["Views"] = views + 123 * tracks.Count;
            return View();
        }

        public async Task<IActionResult> Update(Guid id)
        {
            var author = await _databaseContext.Authors.Where(p => p.Id != ResourceConst.AnonymousAuthor).Where(p => p.Id == id).AsNoTracking().FirstOrDefaultAsync();

            if (author == null)
            {
                _notyfService.Error("Không tìm thấy ca sĩ", 2);
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new AuthorUpdateViewModel
            {
                Id = id,
                Name = author.Name,
                AvatarUrl = !string.IsNullOrWhiteSpace(author.AvatarUrl) ? author.AvatarUrl : ResourceConst.AuthorAvatarDefaultUrl
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AuthorUpdateViewModel author)
        {
            var authorEntity = await _databaseContext.Authors.Where(p => p.Id == author.Id).FirstOrDefaultAsync();
            
            if(authorEntity == null)
            {
                _notyfService.Error("Cập nhật thất bại", 2);
                return RedirectToAction(nameof(Index));
            }

            authorEntity.Name = author.Name;

            if(author.Avatar != null)
            {
                if(!string.IsNullOrWhiteSpace(authorEntity.AvatarUrl) && authorEntity.AvatarUrl != ResourceConst.AuthorAvatarDefaultUrl)
                {
                    System.IO.File.Delete("wwwroot" + authorEntity.AvatarUrl);
                }

                var filePath = $"/images/avatars/{Guid.NewGuid()}.jpg";
                using (var stream = new FileStream("wwwroot" + filePath, FileMode.Create))
                {
                    await author.Avatar.CopyToAsync(stream);
                }
                authorEntity.AvatarUrl = filePath;
            }

            var changeCount = await _databaseContext.SaveChangesAsync();

            if(changeCount > 0)
            {
                _notyfService.Success("Cập nhật thành công", 2);
                return RedirectToAction(nameof(GetDetail), new { id = author.Id });
            }
            _notyfService.Error("Cập nhật thất bại", 2);
            return RedirectToAction(nameof(Update), new { id = author.Id });
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var author = await _databaseContext.Authors.Where(p => p.Id != ResourceConst.AnonymousAuthor).Where(p => p.Id == id).AsNoTracking().FirstOrDefaultAsync();

            return View(author);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmation(Guid id)
        {
            var authors = await _databaseContext.Authors.Where(p => p.Id == id).FirstOrDefaultAsync();

            if(authors == null)
            {
                _notyfService.Error("Cập nhật thất bại", 2);
                return RedirectToAction(nameof(Index));
            }
            _databaseContext.Remove(authors);

            var changeCount = await _databaseContext.SaveChangesAsync();

            if (changeCount > 0)
            {
                _notyfService.Success("Xóa thành công", 2);
                return RedirectToAction(nameof(Index));
            }
            _notyfService.Error("Xóa thất bại", 2);
            return RedirectToAction(nameof(Delete), new { id = authors.Id });
        }

        public IActionResult Add()
        {
            var author = new AuthorAddViewModel();
            author.Name = "";

            return View(author);
        }


        [HttpPost]
        public async Task<IActionResult> Add(AuthorAddViewModel request)
        {
            if(string.IsNullOrWhiteSpace(request.Name))
            {
                _notyfService.Error("Tên ca sĩ không được trống", 2);
                return RedirectToAction(nameof(Add));
            }

            var entity = new Author();
            entity.Name = request.Name;
            entity.AvatarUrl = $"/images/authors/empty.jpg";

            if (request.Avatar != null)
            {
                var fileContentPath = $"/images/authors/{Guid.NewGuid()}.jpg";
                using (var stream = new FileStream("wwwroot" + fileContentPath, FileMode.Create))
                {
                    await request.Avatar.CopyToAsync(stream);
                }
                entity.AvatarUrl = fileContentPath;
            }

            await _databaseContext.Authors.AddAsync(entity);

            var changeCount = await _databaseContext.SaveChangesAsync();

            if (changeCount > 0)
            {
                _notyfService.Success("Thêm thành công", 2);
                return RedirectToAction(nameof(Index));
            }
            _notyfService.Error("Thêm thất bại", 2);
            return RedirectToAction(nameof(GetDetail), new { id = entity.Id });
        }
    }
}
