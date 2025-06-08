using App.Application.Contracts.Infrastructure;
using App.Common.Base;
using App.Domain.Entities;
using App.Infrastructure;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mp3.mvc.Consts;
using mp3.mvc.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace mp3.mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _notyfService;
        private readonly IMediaRepository _mediaRepository;
        private readonly DatabaseContext _databaseContext;


        public HomeController(ILogger<HomeController> logger, INotyfService notyfService, IMediaRepository mediaRepository,
            DatabaseContext databaseContext)
        {
            _logger = logger;
            _notyfService = notyfService;
            _mediaRepository = mediaRepository;
            _databaseContext = databaseContext;
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
        public async Task<IActionResult> Index()
        {
            ViewData["HomePage"] = "text-dark";
            /*ViewData["NewestList"] = await _databaseContext.Media
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.MediaContent)
                .Include(p => p.MediaViewHistory)
                .OrderByDescending(p => p.FavouriteCollections.Count)
                .Take(10)
                .IgnoreAutoIncludes()
                .AsNoTracking()
                .ToListAsync();
*/
            ViewData["NewestList"] = await _mediaRepository.GetSuggestedItemList(getUserId());

            ViewData["TrendingList"] = await _mediaRepository.GetTrendingItemList();

            var items = await _databaseContext.Authors.Where(p => p.Id != ResourceConst.AnonymousAuthor)
                .Select(p => new AuthorSearchItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    AvatarUrl = !string.IsNullOrWhiteSpace(p.AvatarUrl) ? p.AvatarUrl : ResourceConst.AuthorAvatarDefaultUrl
                })
                .ToListAsync();

            // gửi danh sách ca sĩ qua ViewData
            ViewData["AuthorList"] = items;

            return View();
        }
        //[Authorize]
        public async Task<IActionResult> Search(int type = 0, string searchText = "", int page = 1, int pageSize = 8)
        {
            ViewData["ExplorePage"] = "text-dark";
            if (type == 0)
            {
                var query = _databaseContext.Media
                .Include(p => p.MediaContent)
                .Include(p => p.MediaViewHistory)
                .Include(p => p.Author)
                .Include(p => p.Category)
                .AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(p => p.Name.ToLower().Contains(searchText.Trim().ToLower()));
                }


                var total = await query.CountAsync();

                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                ViewBag.Data = new BasePagination<Media>(total, page, pageSize, items);
            }
            else if (type == 1)
            {
                var query = _databaseContext.Authors.AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(p => p.Name.ToLower().Contains(searchText.Trim().ToLower()));
                }


                var total = await query.CountAsync();

                var items = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new AuthorSearchItemViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                    })
                    .ToListAsync();

                foreach (var item in items)
                {
                    var views = await _databaseContext.MediaViewHistory
                        .Include(p => p.Media)
                        .Where(p => p.Media.AuthorId == item.Id)
                        .CountAsync();

                    var numberOfTrack = await _databaseContext.Media.Where(p => p.AuthorId == item.Id).CountAsync();

                    item.Views = views + 123;
                    item.NumberOfTracks = numberOfTrack;
                }

                ViewBag.Data = new BasePagination<AuthorSearchItemViewModel>(total, page, pageSize, items);
            }
            else if (type == 2)
            {
                var query = _databaseContext.Categories.AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(p => p.Name.ToLower().Contains(searchText.Trim().ToLower()));
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

                ViewBag.Data = new BasePagination<CategorySearchItemViewModel>(total, page, pageSize, items);
            }

            var authors = await _databaseContext.Authors.Where(p => p.Id != ResourceConst.AnonymousAuthor)
                .Select(p => new AuthorSearchItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    AvatarUrl = !string.IsNullOrWhiteSpace(p.AvatarUrl) ? p.AvatarUrl : ResourceConst.AuthorAvatarDefaultUrl
                })
                .Where(p => p.Name.ToLower().Contains(searchText.Trim().ToLower()))
                .AsNoTracking()
                .ToListAsync();

            // gửi danh sách ca sĩ qua ViewData
            ViewData["AuthorList"] = authors;

            var categoryMediaQuery = _databaseContext.Categories
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                categoryMediaQuery = categoryMediaQuery.Where(p => p.Name.ToLower().Contains(searchText.Trim().ToLower()));
            }

            var categoryMedia = await categoryMediaQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categoryMediaViewData = new List<CategoryMediaViewModel>();

            foreach (var item in categoryMedia)
            {
                var media = await _databaseContext.Media
                .Include(p => p.MediaContent)
                .Include(p => p.MediaViewHistory)
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Where(p => p.CategoryId == item.Id)
                .Take(5)
                .OrderBy(p => p.UpdatedAt)
                .ToListAsync();

                categoryMediaViewData.Add(new CategoryMediaViewModel
                {
                    Category = item,
                    Medias = media

                });
            }

            // gửi danh sách bài hát theo thể loại
            ViewData["CategoryMedia"] = categoryMediaViewData;

            ViewData["searchText"] = searchText;
            ViewData["Type"] = type;

            return View();
        }

        [HttpPost]
        public IActionResult SearchAction(int type = 0, string searchText = "", int page = 1, int pageSize = 8)
        {
            return RedirectToAction("Search", new { type, searchText, page, pageSize });
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<IActionResult> ListMusic()
        {
            var listMusic = await _databaseContext.Media
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.MediaContent)
                .Include(p => p.MediaViewHistory)
                .OrderByDescending(p => p.MediaViewHistory.Count)
                .Take(50)
                .IgnoreAutoIncludes()
                .AsNoTracking()
                .ToListAsync();

            ViewData["ListMusic"] = listMusic;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MostFavouriteMusic()
        {
            var listMusic = await _databaseContext.Media
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.MediaContent)
                .Include(p => p.MediaViewHistory)
                .OrderByDescending(p => p.FavouriteCollections.Count)
                .Take(40)
                .IgnoreAutoIncludes()
                .AsNoTracking()
                .ToListAsync();

            ViewData["ListMusic"] = listMusic;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MusicDetails()
        {
            ViewData["TrendingList"] = await _mediaRepository.GetTrendingItemList();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MusicTab()
        {
            ViewData["TrendingList"] = await _mediaRepository.GetTrendingItemList();
            ViewData["NewestList"] = await _databaseContext.Media
                .Include(p => p.Author)
                .Include(p => p.Category)
                .Include(p => p.MediaContent)
                .Include(p => p.MediaViewHistory)
                .OrderByDescending(p => p.CreatedAt)
                .Take(50)
                .IgnoreAutoIncludes()
                .AsNoTracking()
                .ToListAsync();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Podcasts()
        {
            ViewData["TrendingList"] = await _mediaRepository.GetTrendingItemList();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> IntroMusic()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Policy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListArtist()
        {
            var items = await _databaseContext.Authors
                .Where(p => p.Id != ResourceConst.AnonymousAuthor)
               .OrderByDescending(p => p.Media.Sum(p => p.MediaViewHistory.Count))
               .Select(p => new AuthorSearchItemViewModel
               {
                   Id = p.Id,
                   Name = p.Name,
                   AvatarUrl = !string.IsNullOrWhiteSpace(p.AvatarUrl) ? p.AvatarUrl : ResourceConst.AuthorAvatarDefaultUrl
               })
               .Take(10)
               .AsNoTracking()
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

            // gửi danh sách ca sĩ qua ViewData
            ViewData["AuthorList"] = items;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AuthorTab()
        {
            var items = await _databaseContext.Authors
                .Where(p => p.Id != ResourceConst.AnonymousAuthor)
               .OrderByDescending(p => p.Media.Sum(p => p.MediaViewHistory.Count))
               .Select(p => new AuthorSearchItemViewModel
               {
                   Id = p.Id,
                   Name = p.Name,
                   AvatarUrl = !string.IsNullOrWhiteSpace(p.AvatarUrl) ? p.AvatarUrl : ResourceConst.AuthorAvatarDefaultUrl
               })
               .Take(10)
               .AsNoTracking()
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

            // gửi danh sách ca sĩ qua ViewData
            ViewData["AuthorList"] = items;

            var items2 = await _databaseContext.Authors.Where(p => p.Id != ResourceConst.AnonymousAuthor)
               .OrderByDescending(p => p.Media.OrderByDescending(p => p.CreatedAt).First() != null ? p.Media.OrderByDescending(p => p.CreatedAt).First().CreatedAt : DateTime.MinValue)
               .Select(p => new AuthorSearchItemViewModel
               {
                   Id = p.Id,
                   Name = p.Name,
                   AvatarUrl = !string.IsNullOrWhiteSpace(p.AvatarUrl) ? p.AvatarUrl : ResourceConst.AuthorAvatarDefaultUrl
               })
               .Take(10)
               .AsNoTracking()
               .ToListAsync();

            foreach (var item in items2)
            {
                var views = await _databaseContext.MediaViewHistory
                    .Include(p => p.Media)
                    .Where(p => p.Media != null && p.Media.AuthorId == item.Id)
                    .CountAsync();

                var numberOfTrack = await _databaseContext.Media.Where(p => p.AuthorId == item.Id).CountAsync();

                item.Views = views;
                item.NumberOfTracks = numberOfTrack;
            }

            // gửi danh sách ca sĩ qua ViewData
            ViewData["NewestList"] = items2;

            return View();
        }
    }
}