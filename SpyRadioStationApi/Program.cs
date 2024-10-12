using Coravel;
using FastEndpoints;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using SpyRadioStationApi.Configurations;
using SpyRadioStationApi.Context;
using SpyRadioStationApi.Extensions;
using SpyRadioStationApi.Handlers;
using SpyRadioStationApi.Implementation.CodeMachines;
using SpyRadioStationApi.Implementation.Repositories;
using SpyRadioStationApi.Implementation.Services;
using SpyRadioStationApi.Interfaces.CodeMachines;
using SpyRadioStationApi.Interfaces.db;
using SpyRadioStationApi.Interfaces.Repositories;
using SpyRadioStationApi.Interfaces.Services;
using SpyRadioStationApi.Jobs;

namespace SpyRadioStationApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddAuthorization();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            builder.Services.AddScheduler();
            builder.Services.AddScoped<ICodeMachine, EnigmaMachine>();
            builder.Services.AddScoped<ICodeService, CodeService>();
            builder.Services.AddScoped<IKeyCodeMachineRepository, KeyCodeMachineRepository>();
            builder.Services.AddScoped<IRadiogramRepository, RadiogramRepository>();
            builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
            builder.Services.AddSingleton<IDatabaseContext, SpyDbContext>();
            builder.Services.AddTransient<PreparingCodeMessageJob>();
            builder.Services.AddTransient<RemoveCodeMessageJob>();
            builder.Services.AddTransient<NotificationJob>();
            builder.Services.AddTransient<CheckPulseJob>();
            builder.Services.AddHttpClient();

            builder.Services.Configure<DbConfiguration>(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    builder.Configuration.GetSection("Database").Bind(options);
                    return;
                }
                options.ConnectionString = Environment.GetEnvironmentVariable("Database_ConnectionString");
                options.Diff = Environment.GetEnvironmentVariable("Database_Diff");
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
                    return;
                }
                options.Key = Environment.GetEnvironmentVariable("Access_Key");
            });
            builder.Services.Configure<Pulse>(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    builder.Configuration.GetSection("Pulse").Bind(options);
                    return;
                }
                options.Address = Environment.GetEnvironmentVariable("Pulse_Address");
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

            app.UseExceptionHandler();

            // configure HTTP request pipeline

            app.UseDatabaseUpdate();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.Services.UseScheduler(scheduler =>
            {
                scheduler.Schedule<CheckPulseJob>()
                   .Cron("*/1 * * * *")
                   .Zoned(TimeZoneInfo.Local)
                   .PreventOverlapping(nameof(CheckPulseJob));

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