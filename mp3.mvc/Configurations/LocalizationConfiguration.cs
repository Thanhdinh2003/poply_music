using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace mp3.mvc.Configurations
{
    public static class LocalizationConfiguration
    {
        public static IServiceCollection AddLocalizationConfiguration(this IServiceCollection services)
        {


            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("vn-VN"),
                    new CultureInfo("en-US"),
                };
                options.DefaultRequestCulture = new RequestCulture("vn-VN");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            return services;
        }
        public static IApplicationBuilder AddLocalizationConfiguration(this IApplicationBuilder app)
        {
            app.UseRequestLocalization();
            return app;
        }
    }
}
