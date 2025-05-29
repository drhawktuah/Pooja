using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Pooja.src.Models.Economy;

public class PoojaEconomyUser
{
    [BsonId]
    public ulong ID { get; set; }
    public long Cash { get; set; } = 0;
    public long Bank { get; set; } = 0;
    public DateTime LastDailyClaim { get; set; } = DateTime.MinValue;
}