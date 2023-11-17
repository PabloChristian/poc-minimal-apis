namespace ParkingLot;

public record CashRegister(Guid Id, DateTime CreatedAt)
{
    public List<CashLineItem> CashLineItems { get; set; } = new();
}

public record CashLineItem(Guid RegisteringUserId, DateTime DateTime, string Description, CashLineItem.CashLineItemType Type, decimal Value)
{
    public enum CashLineItemType
    {
        Open,
        Close,
        Receipt,
    }
}