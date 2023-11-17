using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;

namespace ParkingLot.Features;

public static class SetHourPrice
{
    [HttpPost("/hour-price"), AllowAnonymous]
    public class Endpoint : Endpoint<Request>
    {
        private readonly IMongoDatabase _mongoDatabase;

        public Endpoint(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public override async Task HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var hourPrice = new HourPrice(request.FirstHourPrice, request.OtherHourPrice);

            await _mongoDatabase.GetCollection<HourPrice>("HourPrice").InsertOneAsync(hourPrice, new InsertOneOptions(),  cancellationToken);
            await SendNoContentAsync(cancellationToken);
        }
    }

    public record Request(decimal FirstHourPrice, decimal OtherHourPrice);
}