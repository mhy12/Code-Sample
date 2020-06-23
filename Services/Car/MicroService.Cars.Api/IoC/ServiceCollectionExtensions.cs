using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Cars.Business.Managers;
using Cars.Data.MongoDb.Repositories;
using Cars.Data.Repositories;

namespace Cars.Api.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<ICarRepository, CarRepository>();

            services.AddSingleton<CarManager, CarManager>();

            return services;
        }

        public static IServiceCollection RegisterMessageBroker(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionConfig = configuration.GetSection("MessageBroker");

            services.AddMassTransit(x =>
            {
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri(connectionConfig.GetValue<string>("Host")), hostConfigurator =>
                    {
                        hostConfigurator.Username(connectionConfig.GetValue<string>("Username"));
                        hostConfigurator.Password(connectionConfig.GetValue<string>("Password"));
                    });
                }));

            });

            services.AddMassTransitHostedService();

            return services;
        }
    }
}
