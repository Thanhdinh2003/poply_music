using mp3.mvc.Configurations;
using mp3.mvc.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
var services = builder.Services;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Add services to the container.
services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
}); ;
services.AddSwaggerGen();

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true);

// Add service collection extensions
services.AddServiceCollectionExtensions(builder.Configuration, env);

builder.Services.AddHostedService<CheckExipredPremiumService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.OAuthClientId("swagger");
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PoplyMusic Api V1");
    });
}
app.AddApplicationBuilderExtensions();
app.UseHttpsRedirection();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
