using MongoDB.Bson.Serialization.Attributes;

namespace VehicleSales.Domain.Entities;

public class VehicleSnapshot
{
    [BsonElement("brand")]
    public string Brand { get; set; }

    [BsonElement("model")]
    public string Model { get; set; }

    [BsonElement("year")]
    public int Year { get; set; }

    [BsonElement("color")]
    public string Color { get; set; }

    [BsonElement("originalPrice")]
    public decimal OriginalPrice { get; set; }
}