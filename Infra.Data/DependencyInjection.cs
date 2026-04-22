using Application.UseCases;
using Domain.Interfaces;
using Infra.Data.Context;
using Infra.Data.Repositories;
using Infra.Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                   ?? "Data Source=../rpa.db";

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(connectionString));

            services.AddScoped<ICollectedDataRepository, CollectedDataRepository>();
            services.AddScoped<CollectDataUseCase>();
            services.AddScoped<GetCollectedDataUseCase>();
            services.AddScoped<GetCollectedDataByIdUseCase>();

            services.AddHttpClient<IDataCollectorService, CurrencyCollectorService>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("RpaWorkerService/1.0");
            });

            return services;
        }
    }
}
