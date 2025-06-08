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
using NAudio.Wave;
using System.IO;
using System.Security.Claims;

namespace mp3.mvc.Controllers
{
    public class MediaController : Controller
    {
        private readonly ILogger<MediaController> _logger;
        private readonly INotyfService _notyfService;
        private readonly IMediaRepository _mediaRepository;
        private readonly IMediaViewHistoryRepository _mediaViewHistoryRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly DatabaseContext _databaseContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MediaController(ILogger<MediaController> logger, INotyfService notyfService, IMediaRepository mediaRepository,
            IMediaViewHistoryRepository mediaViewHistoryRepository,
            IAuthorRepository authorRepository,
            DatabaseContext databaseContext, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _notyfService = notyfService;
            _mediaRepository = mediaRepository;
            _authorRepository = authorRepository;
            _databaseContext = databaseContext;
            _mediaViewHistoryRepository = mediaViewHistoryRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        private Guid getUserId()
        {
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                var Idclaim = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier);
                return Guid.Parse(Idclaim!.Value);
            }

            return ResourceConst.AnonymousUser;
        }

        //[Authorize]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var model = await _mediaRepository.GetPurchasedItemList(getUserId());
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Manage(string searchText = "", int page = 1, int pageSize = 10)
        {
            var query = _databaseContext.Media
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Include(p => p.MediaContent).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p => p.Name.ToLower().Contains(searchText.Trim().ToLower()));
            }


            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["searchText"] = searchText;
            ViewBag.Data = new BasePagination<Media>(total, page, pageSize, items);

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Collection(int page = 1, int pageSize = 10)
        {
            ViewData["CollectionPage"] = "text-dark";

            var userId = getUserId();

            var total = await _databaseContext.FavouriteCollections
                .CountAsync(p => p.UserId == userId);

            var idData = await _databaseContext.FavouriteCollections
                .Where(p => p.UserId == userId).Select(p => p.MediaId).Skip((page - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            var query = _databaseContext.Media
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Include(p => p.MediaContent)
                .Where(p => idData.Contains(p.Id))
                .AsQueryable();



            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var model = new BasePagination<Media>(total, page, pageSize, items);

            return View(model);
        }

        public async Task<IActionResult> Play(Guid id)
        {
            var music = await _mediaRepository.GetByIdAsync(id);
            if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated)
            {
                // add view to history
                var newView = new MediaViewHistory();
                newView.MediaId = id;
                newView.UserId = getUserId();

                var isSpam = await _databaseContext.MediaViewHistory
                    .AnyAsync(p => p.UserId == newView.UserId && p.MediaId == newView.MediaId && p.CreatedAt >= DateTime.Now.AddMinutes(-30));
                if (!isSpam)
                {
                    await _mediaViewHistoryRepository.InsertOne(newView);
                }

                ViewBag.isFavouriteMedia = await _databaseContext.FavouriteCollections.AnyAsync(p => p.MediaId == id && p.UserId == getUserId());
            }

            ViewData["TrendingList"] = await _mediaRepository.GetTrendingItemList();
            ViewData["IsPremiumAccount"] = await _databaseContext.Users.Where(p => p.Id == getUserId()).Select(p => p.IsPremiumAccount).FirstOrDefaultAsync();

            return View(music);
        }

        public async Task<IActionResult> Backward(Guid id)
        {
            var current = await _databaseContext.MediaViewHistory.Where(p => p.MediaId == id && p.UserId == getUserId())
                .AsNoTracking()
                .FirstOrDefaultAsync();
            var media = await _databaseContext.MediaViewHistory
                .Where(p => p.MediaId != id && p.UserId == getUserId() && p.CreatedAt < current.CreatedAt)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => p.MediaId).FirstOrDefaultAsync();

            if (media == Guid.Empty)
            {
                return RedirectToAction(nameof(Play), new { id });
            }

            return RedirectToAction(nameof(Play), new { id = media });
        }

        public async Task<IActionResult> Forward(Guid id)
        {
            var current = await _databaseContext.MediaViewHistory.Where(p => p.MediaId == id && p.UserId == getUserId())
                .AsNoTracking()
                .FirstOrDefaultAsync();
            var media = await _databaseContext.MediaViewHistory
                .Where(p => p.MediaId != id && p.UserId == getUserId() && p.CreatedAt > current.CreatedAt)
                .OrderBy(p => p.CreatedAt)
                .Select(p => p.MediaId).FirstOrDefaultAsync();

            if (media != Guid.Empty)
            {
                return RedirectToAction(nameof(Play), new { id = media });
            }

            var historyData = await _databaseContext.MediaViewHistory
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => p.MediaId)
                .Distinct()
                .Take(30)
                .ToListAsync();

            var nextMedia = await _databaseContext.MediaViewHistory
                .GroupBy(p => p.MediaId)
                .Select(p => new { MediaId = p.Key, Count = p.Count() })
                .OrderBy(p => p.Count)
                .Where(p => !historyData.Contains(p.MediaId))
                .Select(p => p.MediaId)
                .FirstOrDefaultAsync();

            if (nextMedia == Guid.Empty)
            {
                nextMedia = await _databaseContext.Media.Where(p => !historyData.Contains(p.Id)).Select(p => p.Id).FirstOrDefaultAsync();
            }

            return RedirectToAction(nameof(Play), new { id = nextMedia });
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            var media = await _databaseContext.Media
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.MediaContent)
                .Where(p => p.Id == id).AsNoTracking().FirstOrDefaultAsync();

            return View(media);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmation(Guid id)
        {
            var mediaContent = await _databaseContext.MediaContents.Where(p => p.MediaId == id).ToListAsync();
            _databaseContext.RemoveRange(mediaContent);

            var mediaHistory = await _databaseContext.MediaViewHistory.Where(p => p.MediaId == id).ToListAsync();
            _databaseContext.RemoveRange(mediaHistory);

            var medias = await _databaseContext.Media.Where(p => p.Id == id).FirstOrDefaultAsync();
            _databaseContext.Remove(medias);

            var changeCount = await _databaseContext.SaveChangesAsync();

            if (changeCount > 0)
            {
                _notyfService.Success("Xóa thành công", 2);
                return RedirectToAction(nameof(Manage));
            }
            _notyfService.Error("Xóa thất bại", 2);
            return RedirectToAction(nameof(Delete), new { id = medias.Id });
        }
        public async Task<IActionResult> Add()
        {
            var media = new MediaAddViewModel();
            ViewBag.CategoryData = await _databaseContext.Categories
                .Select(p => new CategorySearchItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                })
                .ToListAsync();

            ViewBag.AuthorData = await _databaseContext.Authors
                .Select(p => new AuthorSearchItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                })
                .ToListAsync();
            return View(media);
        }


        [HttpPost]
        public async Task<IActionResult> Add(MediaAddViewModel request)
        {
            var id = Guid.NewGuid();
            var filePath = $"/media/audio/{id}.mp3";
            var mediaEntity = new Media
            {
                Id = id,
                Name = request.Name,
                Description = request.Description,
                AuthorId = request.AuthorId,
                UserId = getUserId(),
                CategoryId = request.CategoryId,
                ContentUrl = filePath
            };
            // Handle file uploads
            if (request.ContentFile != null)
            {
                using (var stream = new FileStream("wwwroot" + filePath, FileMode.Create))
                {
                    await request.ContentFile.CopyToAsync(stream);
                }
            }

            await _databaseContext.AddAsync(mediaEntity);
            foreach (var item in request.Avatar)
            {
                var mediaContentId = Guid.NewGuid();
                var fileContentPath = $"/images/media/{mediaContentId}.jpg";
                var mediaContent = new MediaContent
                {
                    Id = mediaContentId,
                    MediaId = id,
                    Value = fileContentPath
                };

                await _databaseContext.AddAsync(mediaContent);

                var fullFilePath = "wwwroot" + fileContentPath;

                using (var stream = new FileStream("wwwroot" + fileContentPath, FileMode.Create))
                {
                    await item.CopyToAsync(stream);
                }

                // check bitrate
                using (var reader = new Mp3FileReader(fullFilePath))
                {
                    var bitrate = reader.Mp3WaveFormat.AverageBytesPerSecond * 8 / 1000;

                    if (bitrate < 128)
                    {
                        _notyfService.Error("Thêm thất bại, tệp âm thanh không đủ tiêu chuẩn", 2);
                        return RedirectToAction(nameof(Manage));
                    }
                }

            }

            var changeCount = await _databaseContext.SaveChangesAsync();

            if (changeCount > 0)
            {
                _notyfService.Success("Thêm thành công", 2);
                return RedirectToAction(nameof(Manage));
            }
            _notyfService.Error("Thêm thất bại", 2);
            return RedirectToAction(nameof(Manage));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            var mediaEntity = await _databaseContext.Media
                .Include(p => p.Category)
                .Include(p => p.Author)
                .Include(p => p.MediaContent).Where(p => p.Id == id).AsNoTracking().FirstOrDefaultAsync();

            var mediaModel = new MediaUpdateViewModel
            {
                Id = mediaEntity.Id,
                Name = mediaEntity.Name,
                Description = mediaEntity.Description,
                AuthorId = mediaEntity.AuthorId,
                CategoryId = mediaEntity.CategoryId
            };
            ViewBag.CategoryData = await _databaseContext.Categories
                .Select(p => new CategorySearchItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                })
                .ToListAsync();

            ViewBag.AuthorData = await _databaseContext.Authors
                .Select(p => new AuthorSearchItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                })
                .ToListAsync();

            return View(mediaModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(MediaUpdateViewModel request)
        {
            var mediaEntity = await _databaseContext.Media.Where(p => p.Id == request.Id).FirstOrDefaultAsync();
            mediaEntity.Name = request.Name;
            mediaEntity.Description = request.Description;
            mediaEntity.AuthorId = request.AuthorId;
            mediaEntity.CategoryId = request.CategoryId;

            if (request.ContentFile != null)
            {
                System.IO.File.Delete("wwwroot" + mediaEntity.ContentUrl);
                var filePath = $"/media/audio/{mediaEntity.Id}.mp3";
                using (var stream = new FileStream("wwwroot" + filePath, FileMode.Create))
                {
                    await request.ContentFile.CopyToAsync(stream);
                }
            }

            _databaseContext.Update(mediaEntity);

            if (request.Avatar != null && request.Avatar.Any())
            {
                var mediaContents = await _databaseContext.MediaContents.Where(p => p.MediaId == request.Id).ToListAsync();
                if (mediaContents.Any())
                {
                    _databaseContext.RemoveRange(mediaContents);
                }

                foreach (var item in request.Avatar)
                {
                    var mediaContentId = Guid.NewGuid();
                    var fileContentPath = $"/images/media/{mediaContentId}.jpg";

                    var mediaContent = new MediaContent
                    {
                        Id = mediaContentId,
                        MediaId = mediaEntity.Id,
                        Value = fileContentPath
                    };

                    await _databaseContext.AddAsync(mediaContent);

                    using (var stream = new FileStream("wwwroot" + fileContentPath, FileMode.Create))
                    {
                        await item.CopyToAsync(stream);
                    }
                }

            }

            var changeCount = await _databaseContext.SaveChangesAsync();

            if (changeCount > 0)
            {
                _notyfService.Success("Cập nhật thành công", 2);
                return RedirectToAction(nameof(Manage));
            }
            _notyfService.Success("Cập nhật thất bại", 2);
            return RedirectToAction(nameof(Update), new { id = request.Id });
        }

        public async Task<IActionResult> History(int page = 1, int pageSize = 10)
        {
            var userId = getUserId();
            var query = _databaseContext.MediaViewHistory
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Include(p => p.Media).ThenInclude(p => p.Category)
                .Include(p => p.Media).ThenInclude(p => p.Author)
                .Include(p => p.Media).ThenInclude(p => p.MediaContent)
                .Select(p => p.Media)
                .Distinct()
                .AsQueryable();

            var total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Data = new BasePagination<Media>(total, page, pageSize, items);

            return View();
        }
        [Authorize]
        public async Task<IActionResult> Like(Guid id)
        {
            var favour = new FavouriteCollection
            {
                UserId = getUserId(),
                MediaId = id
            };

            await _databaseContext.AddAsync(favour);

            await _databaseContext.SaveChangesAsync();

            _notyfService.Success("Thêm vào danh sách yêu thích thành công", 2);
            return RedirectToAction(nameof(Play), new { id });

        }
        [Authorize]
        public async Task<IActionResult> Unlike(Guid id)
        {
            var favour = await _databaseContext.FavouriteCollections.FirstOrDefaultAsync(p => p.MediaId == id && p.UserId == getUserId());

            _databaseContext.Remove(favour);

            await _databaseContext.SaveChangesAsync();

            _notyfService.Success("Xóa khỏi danh sách yêu thích thành công", 2);
            return RedirectToAction(nameof(Play), new { id });

        }

        [HttpGet]
        public async Task<IActionResult> Premium()
        {
            // check nếu user hiện tại đã là account premium
            ViewData["IsPremiumAccount"] = await _databaseContext.Users.Where(p => p.Id == getUserId())
                .Select(p => p.IsPremiumAccount).FirstOrDefaultAsync();

            // check nếu user hiện tại đang gửi request upgrade
            ViewData["IsSendingRequest"] = await _databaseContext.PremiumUpgradeRequests.Where(p => p.UserId == getUserId() && !p.IsAccepted).AnyAsync();

            return View();
        }
    }
}
