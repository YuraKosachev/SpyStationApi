
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
using System.Reflection;

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
            builder.Services.AddTransient<DatabaseActualizationJob>();
            builder.Services.AddHttpClient();

            builder.Services.Configure<DbConfiguration>(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    builder.Configuration.GetSection("Database").Bind(options);
                    return;
                }
                options.Diff = Environment.GetEnvironmentVariable("DB_Diff");
                options.DatabaseName = Environment.GetEnvironmentVariable("DB_DatabaseName");
                options.DbFolder = Environment.GetEnvironmentVariable("DB_Folder");

            });
            builder.Services.Configure<Telegram>(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    builder.Configuration.GetSection("Telegram").Bind(options);
                    return;
                }
                options.Token = Environment.GetEnvironmentVariable("Telegram_Token");
                options.ChatId = Environment.GetEnvironmentVariable("Telegram_ChatId") != null
                ? int.Parse(Environment.GetEnvironmentVariable("Telegram_ChatId"))
                : default;
            });
            builder.Services.Configure<Access>(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    builder.Configuration.GetSection("Access").Bind(options);
                }
                options.Key = Environment.GetEnvironmentVariable("Access_Key");


            });

            builder.Services.AddFastEndpoints();
            builder.Services.AddEndpointsApiExplorer();

            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddSwaggerGen(option =>
                {
                    var info = new OpenApiInfo
                    {
                        Title = "SpyStationApi",
                        Version = "v1"
                    };
                    option.SwaggerDoc("v1", info);
                });
            }

            var app = builder.Build();

            app.UseDatabaseUpdate();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.Services.UseScheduler(scheduler =>
            {
                scheduler.Schedule<PreparingCodeMessageJob>()
                    .Cron("0 * * * *")
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

                scheduler.Schedule<DatabaseActualizationJob>()
                    .Cron("*/1 * * * *")
                    .Zoned(TimeZoneInfo.Local)
                    .PreventOverlapping(nameof(DatabaseActualizationJob));

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
