
using FastEndpoints;
using Microsoft.OpenApi.Models;
using SpyRadioStationApi.Interfaces.CodeMachines;
using SpyRadioStationApi.Implementation.CodeMachines;
using SpyRadioStationApi.Implementation.Services;
using SpyRadioStationApi.Interfaces.Services;
using SpyRadioStationApi.Interfaces.db;
using SpyRadioStationApi.Implementation.db;
using SpyRadioStationApi.Extensions;
using Coravel;
using SpyRadioStationApi.Jobs;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Implementation.Repositories;
using SpyRadioStationApi.Implementation.Handlers;
using SpyRadioStationApi.Models.db;
using SpyRadioStationApi.Configurations;
using Microsoft.AspNetCore.HttpOverrides;

namespace SpyRadioStationApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddAuthorization();

            builder.Services.AddScheduler();
            builder.Services.AddScoped<ICodeMachine, EnigmaMachine>();
            builder.Services.AddScoped<ICodeService, CodeService>();
            builder.Services.AddScoped<IKeyCodeMachineRepository, KeyCodeMachineRepository>();
            builder.Services.AddScoped<IRadiogramRepository, RadiogramRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();
            builder.Services.AddTransient<PreparingCodeMessageJob>();
            builder.Services.AddTransient<RemoveCodeMessageJob>();
            builder.Services.AddTransient<NotificationJob>();
            builder.Services.AddHttpClient();

            builder.Services.Configure<DbConfiguration>(options => builder.Configuration.GetSection("Database").Bind(options));
            builder.Services.Configure<Telegram>(options => builder.Configuration.GetSection("Telegram").Bind(options));
            builder.Services.Configure<Access>(options => builder.Configuration.GetSection("Access").Bind(options));

            builder.Services.AddFastEndpoints();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                var info = new OpenApiInfo
                {
                    Title = "SpyStationApi",
                    Version = "v1"
                };
                option.SwaggerDoc("v1", info);
            });
            var app = builder.Build();

            app.UseDatabaseUpdate();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.Services.UseScheduler(scheduler =>
            {
                scheduler.Schedule<PreparingCodeMessageJob>()
                    .Cron("0 */5 * * *")
                    .Zoned(TimeZoneInfo.Local)
                    .PreventOverlapping(nameof(PreparingCodeMessageJob));

                scheduler.Schedule<RemoveCodeMessageJob>()
                    .Cron("0 1 * * *")
                    .Zoned(TimeZoneInfo.Local)
                    .PreventOverlapping(nameof(RemoveCodeMessageJob));

                scheduler.Schedule<NotificationJob>()
                    .Cron("*/2 * * * *")
                    .Zoned(TimeZoneInfo.Local)
                    .PreventOverlapping(nameof(NotificationJob));

            });
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseFastEndpoints();
            app.Run();


        }
    }
}
