using App.Application.Contracts.Infrastructure;
using App.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace App.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // register automapper

            // di
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMediaRepository, MediaRepository>();
            services.AddScoped<IMediaViewHistoryRepository, MediaViewHistoryRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();

            // register mediatR


            return services;

        }
    }
}
