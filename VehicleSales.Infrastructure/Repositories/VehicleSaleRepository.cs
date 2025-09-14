using MongoDB.Driver;
using VehicleSales.Domain.Entities;
using VehicleSales.Domain.Interfaces;

namespace VehicleSales.Infrastructure.Repositories;

public class VehicleSaleRepository(IMongoDatabase database) : IVehicleSaleRepository
{
    private readonly IMongoCollection<VehicleSale> _collection = database.GetCollection<VehicleSale>("vehicle_sales");

    public async Task<VehicleSale> CreateAsync(VehicleSale sale)
    {
        await _collection.InsertOneAsync(sale);
        return sale;
    }

    public async Task<VehicleSale?> GetByIdAsync(Guid id)
    {
        return await _collection.Find(s => s.Id == id).FirstOrDefaultAsync();
    }

    public async Task<VehicleSale?> GetByPaymentCodeAsync(string paymentCode)
    {
        return await _collection.Find(s => s.PaymentCode == paymentCode).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<VehicleSale>> GetByBuyerCpfAsync(string cpf)
    {
        return await _collection.Find(s => s.BuyerCpf == cpf).ToListAsync();
    }

    public async Task<IEnumerable<VehicleSale>> GetAllAsync()
    {
        return await _collection.Find(_ => true).SortByDescending(s => s.CreatedAt).ToListAsync();
    }

    public async Task UpdateAsync(VehicleSale sale)
    {
        await _collection.ReplaceOneAsync(s => s.Id == sale.Id, sale);
    }
}