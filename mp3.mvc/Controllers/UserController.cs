using App.Application.Contracts.Infrastructure;
using App.Common.Base;
using App.Common.Helpers;
using App.Domain.Entities;
using App.Infrastructure;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mp3.mvc.Consts;
using mp3.mvc.Models;
using System.Security.Claims;

namespace mp3.mvc.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly INotyfService _notyfService;
        private readonly IUserRepository _userRepository;
        private readonly DatabaseContext _databaseContext;
        public UserController(ILogger<UserController> logger, INotyfService notyfService, IUserRepository userRepository,
            DatabaseContext databaseContext)
        {
            _logger = logger;
            _notyfService = notyfService;
            _userRepository = userRepository;
            _databaseContext = databaseContext;
        }
        private Guid getUserId()
        {
            var Idclaim = HttpContext.User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier);
            return Guid.Parse(Idclaim!.Value);
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                var user = await _userRepository.Login(model.username, TokenHelper.md5_hash(model.password));

                if (user.IsLocked)
                {
                    _notyfService.Error("Tài khoản của bạn đã bị vô hiệu hóa", 2);
                    return View();
                }

                // create claims
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username!),
                    new Claim(ClaimTypes.NameIdentifier, user.Id!.ToString(), ClaimValueTypes.String),
                    new Claim("avatarUrl", user.AvatarUrl, ClaimValueTypes.String)
                };

                // check if user is admin
                if (user.IsAdmin)
                {
                    claims.Add(new Claim("role", "admin"));
                }
                else if (user.IsPremiumAccount)
                {
                    claims.Add(new Claim("role", "premium"));
                }
                else
                {
                    claims.Add(new Claim("role", "user"));
                }

                // create identity
                ClaimsIdentity identity = new ClaimsIdentity(claims, "cookie");

                // create principal
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                // sign-in
                await HttpContext.SignInAsync(
                        scheme: "CookieSecurityScheme",
                        principal: principal,
                        properties: new AuthenticationProperties
                        {
                            IsPersistent = model.isRememeberMe, // for 'remember me' feature
                        });
                _logger.LogDebug($"User with username: {model.username} logined.");
                _notyfService.Success("Đăng nhập thành công.", 2);

                return Redirect(model.RequestPath ?? "/home");
            }
            catch (Exception ex)
            {
                _notyfService.Error("Tên đăng nhập không tồn tại hoặc mật khẩu không đúng", 2);
                _logger.LogError(ex.Message);
                return View();
            }
        }
        public async Task<IActionResult> Logout(string requestPath)
        {
            await HttpContext.SignOutAsync(
                    scheme: "CookieSecurityScheme");

            return RedirectToAction("Login");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var newUser = new User
                {
                    Username = model.username,
                    DisplayName = model.displayName,
                    AvatarUrl = "/images/avatars/empty.jpg",
                    Password = TokenHelper.md5_hash(model.password!),
                    Address = model.address,
                    Gender = model.gender,
                    Dob = model.dob,
                };

                await _userRepository.InsertOne(newUser);

                // create claims
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.username),
                    new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString(), ClaimValueTypes.String),
                    new Claim("avatarUrl", newUser.AvatarUrl, ClaimValueTypes.String)
                };

                // create identity
                ClaimsIdentity identity = new ClaimsIdentity(claims, "cookie");

                // create principal
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                // sign-in
                await HttpContext.SignInAsync(
                        scheme: "CookieSecurityScheme",
                        principal: principal);
                _logger.LogDebug($"User with username: {model.username} logined.");
                _notyfService.Success("Đăng ký thành công.");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction("Error", "Home");
            }

        }
        public IActionResult SetLanguage()
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("vn-VN")),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return RedirectToAction("index", "home");
        }
        [Authorize]
        public async Task<IActionResult> GetDetailProfile(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return View(user);
        }

        public async Task<IActionResult> Manage(string searchUser = "", int page = 1, int pageSize = 10)
        {
            var query = _databaseContext.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchUser))
            {
                query = query.Where(p => 
                    (!string.IsNullOrWhiteSpace(p.DisplayName) && p.DisplayName.ToLower().Contains(searchUser.Trim().ToLower())) || 
                    (!string.IsNullOrWhiteSpace(p.Username) && p.Username.ToLower().Contains(searchUser.Trim().ToLower())) || 
                    (!string.IsNullOrWhiteSpace(p.Email) && p.Email.ToLower().Contains(searchUser.Trim().ToLower()))  
                 );
            }
            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            ViewData["searchUser"] = searchUser;
            ViewBag.Data = new BasePagination<User>(total, page, pageSize, items);

            return View();
        }
        public async Task<IActionResult> Activate(Guid id)
        {
            var user = await _databaseContext.Users.Where(p => p.Id == id).FirstOrDefaultAsync();

            user.IsLocked = false;
            _databaseContext.Update(user);
            var changeCount = await _databaseContext.SaveChangesAsync();
            if (changeCount > 0)
            {
                _notyfService.Success($"Khôi phục thành công người dùng {user.Username}", 2);
                return RedirectToAction(nameof(Manage));
            }
            _notyfService.Error($"Khôi phục thất bại người dùng {user.Username}", 2);
            return RedirectToAction(nameof(Manage));
        }

        public async Task<IActionResult> Deactivate(Guid id)
        {
            var user = await _databaseContext.Users.Where(p => p.Id == id).FirstOrDefaultAsync();

            user.IsLocked = true;
            _databaseContext.Update(user);
            var changeCount = await _databaseContext.SaveChangesAsync();
            if (changeCount > 0)
            {
                _notyfService.Success($"Vô hiệu hóa thành công người dùng {user.Username}", 2);
                return RedirectToAction(nameof(Manage));
            }
            _notyfService.Error($"Vô hiệu hóa thất bại người dùng {user.Username}", 2);
            return RedirectToAction(nameof(Manage));
        }

        public async Task<IActionResult> Update(Guid id)
        {
            var user = await _databaseContext.Users.Where(p => p.Id == id).AsNoTracking().FirstOrDefaultAsync();


            var res = new UserUpdateViewModel
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Username = user.Username,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Dob = user.Dob,
                Gender = user.Gender,
                Address = user.Address,
                IsLocked = user.IsLocked,
            };


            return View(res);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateConfirmation(UserUpdateViewModel request)
        {
            var user = await _databaseContext.Users.Where(p => p.Id == request.Id).AsNoTracking().FirstOrDefaultAsync();

            user.DisplayName = request.DisplayName ?? user.DisplayName;
            user.Username = request.Username ?? user.Username;
            user.Email = request.Email ?? user.Email;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.Address = request.Address ?? user.Address;
            user.Gender = request.Gender;
            user.Dob = request.Dob;
            user.IsLocked = request.IsLocked;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                user.Password = TokenHelper.md5_hash(request.Password);
            }

            if(request.Avatar != null)
            {
                if(user.AvatarUrl != ResourceConst.UserAvatarDefaultUrl)
                {
                    System.IO.File.Delete("wwwroot" + user.AvatarUrl);
                }
                
                var filePath = $"/images/avatars/{user.Id}.jpg";
                using (var stream = new FileStream("wwwroot" + filePath, FileMode.Create))
                {
                    await request.Avatar.CopyToAsync(stream);
                }
                user.AvatarUrl = filePath;
            }

            _databaseContext.Update(user);
            var changeCount = await _databaseContext.SaveChangesAsync();
            if (changeCount > 0)
            {
                _notyfService.Success($"Chỉnh sửa thành công thông tin người dùng {user.Username}", 2);
                return RedirectToAction(nameof(Manage));
            }
            _notyfService.Error($"Chỉnh sửa thất bại thông tin người dùng {user.Username}", 2);
            return RedirectToAction(nameof(Update), new {id = request.Id});
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        // user gửi yêu cầu nâng cấp tài khoản
        // sau khi gửi xong, user quay lại màn media/premium nhưng ẩn nút gửi yêu cầu
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendUpgradePremiumRequest()
        {
            var upgradeRequest = await _databaseContext.PremiumUpgradeRequests.Where(p => p.UserId == getUserId() && p.IsAccepted == false).FirstOrDefaultAsync();

            if(upgradeRequest != null)
            {
                upgradeRequest.ExpiredDate = DateTime.Now.AddMonths(3);
            }
            else
            {
                upgradeRequest = new PremiumUpgradeRequest
                {
                    UserId = getUserId(),
                    ExpiredDate = DateTime.Now.AddMonths(3)
                };

                await _databaseContext.PremiumUpgradeRequests.AddAsync(upgradeRequest);
            }

            await _databaseContext.SaveChangesAsync();

            _notyfService.Success("Gửi yêu cầu đăng ký tài khoản Premium thành công.");

            return RedirectToAction("Index", "Home");

            //return View("Media/Premium");
        }

        // xem danh sách nâng cấp tài khoản, dành cho admin, nằm riêng ở tab quản lý giống user, bài hát,..
        [Authorize]
        public async Task<IActionResult> GetListUpgradePremiumRequest(string searchUser = "", int page = 1, int pageSize = 10)
        {
            var query = _databaseContext.PremiumUpgradeRequests.Include(p => p.User).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchUser))
            {
                query = query.Where(p => !string.IsNullOrWhiteSpace(p.User.Username) && p.User.Username.Contains(searchUser.Trim().ToLower()));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["searchUser"] = searchUser;
            ViewBag.Data = new BasePagination<PremiumUpgradeRequest>(total, page, pageSize, items);

            return View();
        }

        // phê duyệt yêu cầu
        // phê duyệt xong quay về màn xem danh sách
        public async Task<IActionResult> AcceptRequest(Guid id)
        {
            var upgradeRequest = await _databaseContext.PremiumUpgradeRequests.Where(p => p.Id == id).FirstOrDefaultAsync();

            upgradeRequest.IsAccepted = true;

            var user = await _databaseContext.Users.FirstOrDefaultAsync(p => p.Id == upgradeRequest.UserId);
            user.IsPremiumAccount = true;

            await _databaseContext.SaveChangesAsync();

            _notyfService.Success("Nâng cấp tài khoản thành công.");

            return RedirectToAction("GetListUpgradePremiumRequest");
        }
    }
}
