using Serilog;

namespace WebApi.Configurations
{
    public static class LoggerConfiguraton
    {
        public static void AddSerilogConfiguration(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)

                .CreateLogger();
        }
    }
}
