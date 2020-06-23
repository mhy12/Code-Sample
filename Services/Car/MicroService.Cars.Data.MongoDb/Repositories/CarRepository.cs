using MicroService.Common.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using Cars.Data.Models;
using Cars.Data.Repositories;
using System.Threading.Tasks;

namespace Cars.Data.MongoDb.Repositories
{
    public class CarRepository : ICarRepository
    {
        private IMongoDatabase _database;
        private readonly IMongoCollection<Car> _collection;

        public CarRepository(IOptions<MongoDbConnectionSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);

            _database = client.GetDatabase(options.Value.Database);

            _collection = _database.GetCollection<Car>("cars");
        }

        public async Task AddCar(Car car)
        {
            await _collection.InsertOneAsync(car);
        }

        public async Task<List<Car>> GetAllCars()
        {
            var cars = (await _collection.FindAsync<Car>(_ => true)).ToListAsync();

            return await cars;
        }

        public async Task<Car> GetCarById(string carId)
        {
            var filter = Builders<Car>.Filter.Where(cr => cr.Id == carId);

            var car = (await _collection.FindAsync<Car>(filter)).FirstOrDefault();

            return car;
        }

        public async Task DeleteCar(string carId)
        {
            var filter = Builders<Car>.Filter.Where(cr => cr.Id == carId);

            await _collection.DeleteOneAsync(filter);
        }

        public async Task UpdateCar(string carId, Car car)
        {
            var updateFields = Builders<Car>.Update
                                            .Set(cr => cr.Owner, car.Owner)
                                            .Set(cr => cr.PlateId, car.PlateId)
                                            .Set(cr => cr.SerialNumber, car.SerialNumber);

            await _collection.FindOneAndUpdateAsync(cr => cr.Id == carId, updateFields);
        }
    }
}
