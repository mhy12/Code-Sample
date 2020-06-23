using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CarSuppliers.Api.MassTransitConsumers;
using CarSuppliers.Data.MongoDb.Repositories;
using CarSuppliers.Data.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CarSuppliers.Api.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<ICarSupplierRepository, CarSupplierRepository>();

            return services;
        }

        public static IServiceCollection RegisterMessageBroker(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionConfig = configuration.GetSection("MessageBroker");

            services.AddMassTransit(x =>
            {
                x.AddConsumer<UpdateCarListConsumer>();

                x.AddBus(context => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri(connectionConfig.GetValue<string>("Host")), hostConfigurator =>
                    {
                        hostConfigurator.Username(connectionConfig.GetValue<string>("Username"));
                        hostConfigurator.Password(connectionConfig.GetValue<string>("Password"));
                    });

                    cfg.ReceiveEndpoint("UpdateCarListConsumer", ep =>
                    {
                        ep.ConfigureConsumer<UpdateCarListConsumer>(context);
                    });

                }));
            });

            services.AddSingleton<IHostedService, BusService>();

            return services;
        }
    }

    public class BusService : IHostedService
    {
        private readonly IBusControl _busControl;

        public BusService(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _busControl.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _busControl.StopAsync(cancellationToken);
        }
    }
}
