using CarSuppliers.Data.Models;
using System.Threading.Tasks;

namespace CarSuppliers.Data.Repositories
{
    public interface ICarSupplierRepository
    {
        Task AddSupplier(CarSupplier supplier);

        Task DeleteSupplier(string supplierId);

        Task<CarSupplier> GetSupplierById(string supplierKey);

        Task UpdateSupplier(string supplierId, CarSupplier supplier);

        #region CONSUMER METHODS 

        Task AddCarIdToList(string supplierId, string carId);

        #endregion
    }
}
