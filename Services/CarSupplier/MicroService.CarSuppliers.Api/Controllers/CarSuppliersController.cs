using Microsoft.AspNetCore.Mvc;
using CarSuppliers.Data.Models;
using CarSuppliers.Data.Repositories;
using System.Threading.Tasks;

namespace CarSuppliers.Api.Controllers
{
    [Route("api/suppliers")]
    public class CarSuppliersController : Controller
    {
        private ICarSupplierRepository _carSupplierRepo;

        public CarSuppliersController(ICarSupplierRepository carSupplierRepo)
        {
            _carSupplierRepo = carSupplierRepo;
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> AddSupplier([FromForm]CarSupplier supplier)
        {
            await _carSupplierRepo.AddSupplier(supplier);

            return Ok();
        }

        [Route("{supplierId}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateSupplier(string supplierId, [FromForm]CarSupplier supplier)
        {
            await _carSupplierRepo.UpdateSupplier(supplierId, supplier);

            return Ok();
        }

        [Route("{supplierId}")]
        [HttpGet]
        public async Task<IActionResult> GetSupplier(string supplierId)
        {
            var result = await _carSupplierRepo.GetSupplierById(supplierId);

            return Ok(result);
        }

        [Route("{supplierId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteSupplier(string supplierId)
        {
            await _carSupplierRepo.DeleteSupplier(supplierId);

            return Ok();
        }
    }
}
