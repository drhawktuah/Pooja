using MongoDB.Bson.Serialization.Attributes;

namespace Pooja.src.Models.General;

public class PoojaAdmin
{
    [BsonId]
    public required ulong ID { get; set; }
    public required string Name { get; set; }
    public required PoojaHierarchy Position { get; set; }
}
