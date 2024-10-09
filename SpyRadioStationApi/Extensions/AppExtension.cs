using SpyRadioStationApi.Interfaces.db;

namespace SpyRadioStationApi.Extensions
{
    public static class AppExtension
    {
        public static IApplicationBuilder UseDatabaseUpdate(this IApplicationBuilder app)
        { 
            var dataBaseBootstrap = app.ApplicationServices.GetService<IDatabaseBootstrap>();
            dataBaseBootstrap?.Setup();
            return app;

        }
    }
}
