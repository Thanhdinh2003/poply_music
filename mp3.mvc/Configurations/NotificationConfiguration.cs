using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;

namespace mp3.mvc.Configurations
{
    public static class NotificationConfiguration
    {
        public static IServiceCollection AddNotificationConfiguration(this IServiceCollection services)
        {
            services.AddNotyf(config =>
            {
                config.DurationInSeconds = 8;
                config.IsDismissable = true;
                config.Position = NotyfPosition.TopRight;
            });
            return services;
        }
        public static IApplicationBuilder AddNotificationConfiguration(this IApplicationBuilder app)
        {
            app.UseNotyf();
            return app;
        }
    }
}
