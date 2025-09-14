using VehicleSales.Domain.Interfaces;
using VehicleSales.Infrastructure.Data;

namespace VehicleSales.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IVehicleRepository _vehicleRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IVehicleRepository Vehicles => 
        _vehicleRepository ??= new VehicleRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
