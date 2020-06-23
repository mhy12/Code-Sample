using Cars.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cars.Data.Repositories
{
    public interface ICarRepository
    {
        Task AddCar(Car car);

        Task DeleteCar(string car);

        Task<List<Car>> GetAllCars();

        Task<Car> GetCarById(string carId);

        Task UpdateCar(string carId, Car car);
    }
}
