using MongoDB.Driver;

namespace ParkingLot;

public class HourPriceProvider : IHourPriceProvider
{
    private readonly IMongoDatabase _mongoDatabase;

    public HourPriceProvider(IMongoDatabase mongoDatabase)
    {
        _mongoDatabase = mongoDatabase;
    }

    public async Task<HourPrice?> GetHourPrice(CancellationToken cancellationToken)
    {
        var collectionQuery = await _mongoDatabase.GetCollection<HourPrice>("HourPrice").FindAsync(FilterDefinition<HourPrice>.Empty,
            cancellationToken: cancellationToken);

        return collectionQuery.FirstOrDefault(cancellationToken);
    }
}