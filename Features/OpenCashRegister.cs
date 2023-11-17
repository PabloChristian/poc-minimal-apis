using FastEndpoints;
using MongoDB.Driver;

namespace ParkingLot.Features;

public static class OpenCashRegister
{
    public class Endpoint : Endpoint<Request, Response>
    {
        private readonly ILoggedUserAccessor _loggedUserAccessor;
        private readonly IMongoDatabase _db;

        public Endpoint(ILoggedUserAccessor loggedUserAccessor, IMongoDatabase db)
        {
            _loggedUserAccessor = loggedUserAccessor;
            _db = db;
        }

        public override void Configure()
        {
            Post("/cash-register");
        }
        
        public override async Task HandleAsync(Request req, CancellationToken ct)
        {
            var lastCashRegister = await _db.GetCollection<CashRegister>("CashRegisters")
                .Find(FilterDefinition<CashRegister>.Empty)
                .SortByDescending(x => x.CreatedAt)
                .Limit(1)
                .FirstOrDefaultAsync(ct); 

            //Check if the cash register is currently closed
            if (lastCashRegister is not null && lastCashRegister.CashLineItems.Last().Type != CashLineItem.CashLineItemType.Close)
            {
                AddError("Cash register is already open");
                await SendErrorsAsync(cancellation: ct);
            }
            
            var cashRegister = new CashRegister(Guid.NewGuid(), DateTime.UtcNow);

            var cashLineItem = new CashLineItem(
                _loggedUserAccessor.Get().Id,
                DateTime.UtcNow,
                "Open cash register",
                CashLineItem.CashLineItemType.Open,
                req.InCashValue!.Value);
    
            cashRegister.CashLineItems.Add(cashLineItem);

            await _db.GetCollection<CashRegister>("CashRegisters").InsertOneAsync(cashRegister, new InsertOneOptions(), ct);
            
            await SendAsync(new Response(cashRegister.Id, cashRegister.CreatedAt), 200, ct);
        }
    }

    public record Request(decimal? InCashValue);
    
    public record Response(Guid Id, DateTime CreateAt);
    
    public class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.InCashValue).NotEmpty();
        }
    }
}