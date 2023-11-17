using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace ParkingLot.Configurations;

public static  class MongoDbConfig
{
    public static void AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var conventionPack = new ConventionPack();
        conventionPack.Add(new UtcIsoDateTimeConvention());
        
        ConventionRegistry.Register("DateTimeConvention", conventionPack, _ => true);

        services.AddTransient<IMongoDatabase>(_ =>
            new MongoClient(configuration.GetConnectionString("Mongo")).GetDatabase("ParkingLot"));
    }
    
    class UtcIsoDateTimeConvention : ConventionBase, IMemberMapConvention
    {
        public void Apply(BsonMemberMap memberMap)
        {
            if (memberMap.MemberType == typeof(DateTime))
            {
                memberMap.SetSerializer(new UtcIsoDateTimeSerializer());
            }
        }
    }
    
    class UtcIsoDateTimeSerializer : DateTimeSerializer
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DateTime value)
        {
            var utcValue = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            base.Serialize(context, args, utcValue);
        }
    }
}