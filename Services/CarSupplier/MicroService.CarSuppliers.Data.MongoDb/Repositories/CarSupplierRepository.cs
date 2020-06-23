using Microsoft.Extensions.Options;
using MongoDB.Driver;
using CarSuppliers.Data.Models;
using CarSuppliers.Data.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroService.Common.Models;

namespace CarSuppliers.Data.MongoDb.Repositories
{
    public class CarSupplierRepository : ICarSupplierRepository
    {
        private IMongoDatabase _database;
        private readonly IMongoCollection<CarSupplier> _collection;

        public CarSupplierRepository(IOptions<MongoDbConnectionSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            
            _database = client.GetDatabase(options.Value.Database);

            _collection = _database.GetCollection<CarSupplier>("carSupplier");
        }

        public async Task AddSupplier(CarSupplier supplier)
        {
            if (supplier.CarIds == null)
                supplier.CarIds = new List<string>();

            await _collection.InsertOneAsync(supplier);
        }

        public async Task DeleteSupplier(string supplierId)
        {
            var filter = Builders<CarSupplier>.Filter.Where(supplier => supplier.Id == supplierId);

            await _collection.DeleteOneAsync(filter);
        }

        public async Task<CarSupplier> GetSupplierById(string supplierKey)
        {
            var filter = Builders<CarSupplier>.Filter.Where(supplier => supplier.Id == supplierKey);

            var carSupplier = (await _collection.FindAsync<CarSupplier>(filter)).FirstOrDefault();

            return carSupplier;
        }

        public async Task UpdateSupplier(string supplierId, CarSupplier supplier)
        {
            var updateFields = Builders<CarSupplier>.Update
                                                        .Set(supp => supp.Name, supplier.Name)
                                                        .Set(supp => supp.Website, supplier.Website)
                                                        .Set(supp => supp.ContactName, supplier.ContactName)
                                                        .Set(supp => supp.ContactEmail, supplier.ContactEmail)
                                                        .Set(supp => supp.ContactPhoneNumber, supplier.ContactPhoneNumber);

            await _collection.FindOneAndUpdateAsync(supplier => supplier.Id == supplierId, updateFields);
        }

        public async Task AddCarIdToList(string supplierId, string carId)
        {
            var supplier = await GetSupplierById(supplierId);

            if (supplier != null)
            {
                var update = Builders<CarSupplier>.Update.Push(e => e.CarIds, carId);

                var filter = Builders<CarSupplier>.Filter.Where(supplier => supplier.Id == supplierId);

                await _collection.FindOneAndUpdateAsync(filter, update);
            }           
        }
    }
}
