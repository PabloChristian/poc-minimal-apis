using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;

namespace ParkingLot.Features;

public static class CreateTicket
{
    public class Endpoint : Endpoint<Request, Response>
    {
        private readonly IMongoDatabase _mongoDatabase;

        public Endpoint(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public override void Configure()
        {
            Post("/tickets");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var ticket = new Ticket(Guid.NewGuid(), new CheckIn(request.CheckInTime!.Value, request.LicencePlate!));

            await _mongoDatabase.GetCollection<Ticket>("Tickets").InsertOneAsync(ticket, new InsertOneOptions(), cancellationToken);

            var response = new Response(ticket.Id);

            await SendAsync(response, cancellation: cancellationToken);
        }
    }
    
    public record Request(string? LicencePlate, DateTime? CheckInTime);

    public record Response(Guid TicketId);

    public class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.LicencePlate).NotEmpty();
            RuleFor(x => x.CheckInTime).NotEmpty();
        }
    }
}
