using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using VehicleSales.Domain.Enums;

namespace VehicleSales.Domain.Entities;

public class VehicleSale
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; private set; }

    [BsonElement("vehicleId")]
    public Guid VehicleId { get; private set; }

    [BsonElement("buyerCpf")]
    public string BuyerCpf { get; private set; }

    [BsonElement("buyerName")]
    public string BuyerName { get; private set; }

    [BsonElement("buyerEmail")]
    public string BuyerEmail { get; private set; }

    [BsonElement("salePrice")]
    public decimal SalePrice { get; private set; }

    [BsonElement("paymentCode")]
    public string PaymentCode { get; private set; }

    [BsonElement("paymentStatus")]
    public PaymentStatus PaymentStatus { get; private set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; private set; }

    [BsonElement("updatedAt")]
    public DateTime? UpdatedAt { get; private set; }

    [BsonElement("vehicleData")]
    public VehicleSnapshot VehicleData { get; private set; }

    protected VehicleSale()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        PaymentStatus = PaymentStatus.Pending;
    }

    public VehicleSale(Guid vehicleId, string buyerCpf, string buyerName, string buyerEmail, 
        decimal salePrice, VehicleSnapshot vehicleData) : this()
    {
        VehicleId = vehicleId;
        BuyerCpf = buyerCpf;
        BuyerName = buyerName;
        BuyerEmail = buyerEmail;
        SalePrice = salePrice;
        PaymentCode = GeneratePaymentCode();
        VehicleData = vehicleData;
    }

    public void UpdatePaymentStatus(PaymentStatus status)
    {
        PaymentStatus = status;
        UpdatedAt = DateTime.UtcNow;
    }

    private string GeneratePaymentCode()
    {
        return $"PAY-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }
}