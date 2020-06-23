using MassTransit;
using MicroService.Messaging.Models;
using Microsoft.Extensions.Options;
using CarSuppliers.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarSuppliers.Api.MassTransitConsumers
{
    public class UpdateCarListConsumer : IConsumer<CarInfo>
    {
        private ICarSupplierRepository _batterySupplierRepo;

        public UpdateCarListConsumer(ICarSupplierRepository batterySupplierRepo)
        {
            _batterySupplierRepo = batterySupplierRepo;
        }

        public async Task Consume(ConsumeContext<CarInfo> context)
        {
            var message = context.Message;

            await _batterySupplierRepo.AddCarIdToList(message.SupplierId, message.CarId);
        }
    }
}
