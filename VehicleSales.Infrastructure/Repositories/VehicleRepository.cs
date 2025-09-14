using Microsoft.EntityFrameworkCore;
using VehicleSales.Domain.Entities;
using VehicleSales.Domain.Interfaces;
using VehicleSales.Infrastructure.Data;

namespace VehicleSales.Infrastructure.Repositories;

public class VehicleRepository(ApplicationDbContext context) : IVehicleRepository
{
    public async Task<Vehicle?> GetByIdAsync(Guid id)
    {
        return await context.Vehicles.FindAsync(id);
    }

    public async Task<Vehicle?> GetByPaymentCodeAsync(Guid vehicleId)
    {
        return await context.Vehicles
            .FirstOrDefaultAsync(v => v.Id == vehicleId);
    }

    public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
    {
        return await context.Vehicles
            .Where(v => !v.IsSold)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vehicle>> GetSoldVehiclesAsync()
    {
        return await context.Vehicles
            .Where(v => v.IsSold == true)
            .ToListAsync();
    }

    public async Task<Vehicle> AddAsync(Vehicle Vehicle)
    {
        await context.Vehicles.AddAsync(Vehicle);
        return Vehicle;
    }

    public async Task UpdateAsync(Vehicle Vehicle)
    {
        context.Vehicles.Update(Vehicle);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.Vehicles.AnyAsync(v => v.Id == id);
    }
    
    public async Task DeleteAsync(Vehicle vehicle)
    {
        context.Vehicles.Remove(vehicle);
        await Task.CompletedTask;
    }
}
