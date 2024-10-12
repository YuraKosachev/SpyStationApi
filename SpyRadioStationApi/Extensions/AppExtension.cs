using SpyRadioStationApi.Context;
using SpyRadioStationApi.Interfaces.db;

namespace SpyRadioStationApi.Extensions
{
    public static class AppExtension
    {
       
        public static WebApplication UseDatabaseUpdate(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IDatabaseContext>();
            context.Init().GetAwaiter().GetResult();
            return app;

        }
    }
}
