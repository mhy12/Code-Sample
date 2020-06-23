using MassTransit;
using MicroService.Messaging.Models;
using Cars.Data.Models;
using Cars.Data.Repositories;
using System.Threading.Tasks;
using System;

namespace Cars.Business.Managers
{
    public class CarManager
    {
        private string _busBaseUri = "rabbitmq://localhost";

        private readonly IBus _bus;

        private ICarRepository _carRepo;

        public CarManager(ICarRepository carRepo, IBus bus)
        {
            _bus = bus;

            _carRepo = carRepo;
        }

        public async Task AddCar(Car car)
        {
            await _carRepo.AddCar(car);

            await SynchSupplierDB(car);
        }

        public async Task SynchSupplierDB(Car car)
        {
            string queueName = "/UpdateCarListConsumer";

            Uri fullEndPointUri = new Uri(_busBaseUri + queueName);

            var carInfo = new CarInfo { SupplierId = car.SupplierId, CarId = car.Id };

            var sendEndpoint = await _bus.GetSendEndpoint(fullEndPointUri);

            await sendEndpoint.Send(carInfo);
        }
    }
}
