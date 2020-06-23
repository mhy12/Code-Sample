using Microsoft.AspNetCore.Mvc;
using Cars.Business.Managers;
using Cars.Data.Models;
using Cars.Data.Repositories;
using System.Threading.Tasks;

namespace Cars.Api.Controllers
{
    [Route("api/cars")]
    public class CarsController : Controller
    {
        private ICarRepository _carRepo;

        private CarManager _carManager;

        public CarsController(CarManager carManager, ICarRepository carRepo)
        {
            _carRepo = carRepo;

            _carManager = carManager;
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> AddCar([FromForm]Car car)
        {
            await _carManager.AddCar(car);

            return Ok();
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetAllCars()
        {
            var result = await _carRepo.GetAllCars();

            return Ok(result);
        }

        [Route("{carId}")]
        [HttpPatch]
        public async Task<IActionResult> UpdateCar(string carId, [FromForm]Car car)
        {
            await _carRepo.UpdateCar(carId, car);

            return Ok();
        }

        [Route("{carId}")]
        [HttpGet]
        public async Task<IActionResult> GetCar(string carId)
        {
            var result = await _carRepo.GetCarById(carId);

            return Ok(result);
        }

        [Route("{carId}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteCar(string carId)
        {
            await _carRepo.DeleteCar(carId);

            return Ok();
        }
    }
}
