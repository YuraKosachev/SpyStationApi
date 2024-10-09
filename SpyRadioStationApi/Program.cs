
using FastEndpoints;
using Microsoft.OpenApi.Models;
using SpyRadioStationApi.Interfaces.CodeMachines;
using SpyRadioStationApi.Implementation.CodeMachines;
using SpyRadioStationApi.Implementation.Services;
using SpyRadioStationApi.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using SpyRadioStationApi.Interfaces.db;
using SpyRadioStationApi.Implementation.db;
using SpyRadioStationApi.Extensions;
using Coravel;
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

            builder.Services.AddScheduler();
            builder.Services.AddScoped<ICodeMachine, EnigmaMachine>();
            builder.Services.AddScoped<ICodeService, CodeService>();
            builder.Services.AddSingleton<IDatabaseBootstrap, DatabaseBootstrap>();
            builder.Services.AddTransient<PreparingCodeMessageJob>();

            builder.Services.Configure<DbConfiguration>(options => builder.Configuration.GetSection("Database").Bind(options));

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
            
            app.Services.UseScheduler(scheduler =>
            {
                scheduler.Schedule<PreparingCodeMessageJob>()//.EveryMinute();
                    .Cron("* * * * *")
                    .Zoned(TimeZoneInfo.Local)
                    .PreventOverlapping(nameof(PreparingCodeMessageJob));

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
