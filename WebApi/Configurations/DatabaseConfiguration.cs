using App.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using PostgreSQLProjectAssembly = App.PostgreSQL.MigrationAssembly;
using SqlServerSQLProjectAssembly = App.SQLServer.MigrationAssembly;

namespace WebApi.Configurations
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabaseConfiguration<T>(this IServiceCollection services,
            IConfiguration configuration, IHostEnvironment environment)
            where T : DbContext
        {
            string connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new ArgumentException("Missing database connection string.");
            string typeOfDatabase = configuration["DatabaseSettings:DatabaseType"]
                ?? throw new ArgumentException("Missing database type.");


            services.AddDbContext<T>(
                options => _ = typeOfDatabase switch
                {
                    "postgres" =>
                        options.UseNpgsql(
                        connectionString,
                        x => x.MigrationsAssembly(typeof(PostgreSQLProjectAssembly).Assembly.FullName)),
                    "sqlserver" =>
                        options.UseSqlServer(
                        connectionString,
                        x => x.MigrationsAssembly(typeof(SqlServerSQLProjectAssembly).Assembly.FullName)),
                    _ =>
                        options.UseInMemoryDatabase(
                        connectionString)
                });

            bool autoMigration = configuration.GetValue<bool>("DatabaseSettings:AutoMigration");
            if (autoMigration)
            {
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    MigrateDatabase<T>(serviceProvider, environment);
                }
            }

            bool autoSeedData = configuration.GetValue<bool>("DatabaseSettings:AutoSeedData");
            if (autoSeedData)
            {
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    SeedData<T>(serviceProvider);
                }
            }

            return services;
        }
        private static void MigrateDatabase<T>(IServiceProvider serviceProvider, IHostEnvironment environment)
            where T : DbContext
        {
            if (environment.IsDevelopment() || environment.IsProduction())
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<T>();
                    context.Database.OpenConnection();
                    context.Database.Migrate();
                }
            }
        }
        private static void SeedData<T>(IServiceProvider serviceProvider)
            where T : DbContext
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<T>();
                if (!context.Set<User>().Any())
                {
                    context.Set<User>().AddRange(MockData.UserData);
                }

                if (!context.Set<Author>().Any())
                {
                    context.Set<Author>().AddRange(MockData.AuthorData);
                }

                if (!context.Set<Category>().Any())
                {
                    context.Set<Category>().AddRange(MockData.CategoryData);
                }

                if (!context.Set<Media>().Any())
                {
                    context.Set<Media>().AddRange(MockData.MediaData);
                }

                context.SaveChanges();
            }
        }

        public static IApplicationBuilder UseApplicationDatabase<T>(this IApplicationBuilder app)
            where T : DbContext
        {
            return app;
        }
    }
}
