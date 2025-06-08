using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebApi.Configurations
{
    public static class AuthenticationConfiguration
    {
        public static IServiceCollection AddAuthenticationConfiguration(this IServiceCollection services)
        {
            services
                .AddAuthentication("CookieSecurityScheme")
                .AddCookie("CookieSecurityScheme", options =>
                {
                    options.AccessDeniedPath = new PathString("/home/error");
                    options.Cookie = new CookieBuilder
                    {
                        //Domain = "",
                        HttpOnly = true,
                        Name = "cookie",
                        Path = "/",
                        SameSite = SameSiteMode.Lax,
                        SecurePolicy = CookieSecurePolicy.SameAsRequest
                    };
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnSignedIn = context =>
                        {
                            Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                                "OnSignedIn", context.Principal?.Identity?.Name);
                            return Task.CompletedTask;
                        },
                        OnSigningOut = context =>
                        {
                            Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                                "OnSigningOut", context.HttpContext?.User?.Identity?.Name);
                            return Task.CompletedTask;
                        },
                        OnValidatePrincipal = context =>
                        {
                            Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                                "OnValidatePrincipal", context.Principal.Identity.Name);
                            return Task.CompletedTask;
                        }
                    };
                    options.ExpireTimeSpan = TimeSpan.FromDays(10);
                    options.LoginPath = new PathString("/user/login");
                    options.ReturnUrlParameter = "/home";
                    options.SlidingExpiration = true;
                });
            return services;
        }
        public static IApplicationBuilder AddAuthenticationConfiguration(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
