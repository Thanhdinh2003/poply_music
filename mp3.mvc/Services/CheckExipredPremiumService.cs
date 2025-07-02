using App.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace mp3.mvc.Services
{
    public class CheckExipredPremiumService : BackgroundService
    {
        private readonly ILogger<CheckExipredPremiumService> _logger;
        private readonly DatabaseContext _databaseContext;
        public IServiceProvider Services { get; }

        public CheckExipredPremiumService(IServiceProvider services,
                ILogger<CheckExipredPremiumService> logger)
        {
            Services = services;
            _logger = logger;
            using (var scope = services.CreateScope())
            {
                _databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>() as DatabaseContext;
            }
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = Services.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                        var expiredUsers = await dbContext.Users
                            // tìm nhưng tài khoản đang là tài khoản premium
                            .Where(p => p.IsPremiumAccount)
                            // kiểm tra trong 3 tháng gần nhất không có giao dích nào
                            .Where(p => !p.PremiumUpgradeRequests
                                .Any(pur => pur.IsAccepted && pur.CreatedAt.AddMonths(3) > DateTime.UtcNow))
                            .ToListAsync(stoppingToken);

                        foreach (var user in expiredUsers)
                        {
                            user.IsPremiumAccount = false;
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                    }

                    _logger.LogInformation("Checked expired premium users at: {time}", DateTime.UtcNow);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while checking expired premium users.");
                }

                // Delay for 24 hours (or change to whatever interval you want)
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

    }
}
