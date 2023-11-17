using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;

namespace ParkingLot.Features;

public static class RegisterCheckout
{
    [HttpPost("/tickets/{id}/checkout"), AllowAnonymous]
    public class Endpoint : Endpoint<Request, Response>
    {
        private readonly IMongoDatabase _mongoDatabase;
        private readonly IHourPriceProvider _hourPriceProvider;

        public Endpoint(IMongoDatabase mongoDatabase, IHourPriceProvider hourPriceProvider)
        {
            _mongoDatabase = mongoDatabase;
            _hourPriceProvider = hourPriceProvider;
        }

        public override async Task HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var collection = _mongoDatabase.GetCollection<Ticket>("Tickets");

            var ticket = await (await collection
                    .FindAsync(x => x.Id == request.TicketId, cancellationToken: cancellationToken))
                .FirstOrDefaultAsync(cancellationToken);

            if (ticket is null)
            {
                AddError("Ticket", "Ticket not found");
            }
            
            var hourPrice = await _hourPriceProvider.GetHourPrice(cancellationToken);
            if (hourPrice is null)
            {
                AddError("HourPrice", "Hour price is not set");
            }
            
            ThrowIfAnyErrors();
            
            ticket!.Checkout = new Checkout(request.CheckoutTime!.Value, hourPrice!);

            var updateDefinition = Builders<Ticket>.Update.Set(x => x.Checkout, ticket.Checkout);

            await collection.UpdateOneAsync(x => x.Id == ticket.Id, updateDefinition,
                cancellationToken: cancellationToken);

            var response = new Response(ticket.Id, ticket.TotalTime!.Value, ticket.BilledHours, ticket.TotalPrice!.Value);

            await SendAsync(response, 200, cancellationToken);
        }
    }

    public record Request(Guid? TicketId, DateTime? CheckoutTime);

    public record Response(Guid CheckInId, TimeSpan TotalTime, int BilledHours, decimal TotalPrice);

    public class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.TicketId).NotEmpty();
            RuleFor(x => x.CheckoutTime).NotEmpty();
        }
    }
}