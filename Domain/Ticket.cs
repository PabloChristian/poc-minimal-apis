namespace ParkingLot;

public record Ticket(Guid Id, CheckIn CheckIn)
{
    public Checkout? Checkout { get; set; }

    public TimeSpan? TotalTime => Checkout?.CheckOutTime - CheckIn.CheckInTime ?? null;

    public int BilledHours => TotalTime?.TotalHours < 1 ? 1 : (int)Math.Ceiling(TotalTime!.Value.TotalHours);

    public decimal? TotalPrice => Checkout?.HourPrice.CalcTotal(BilledHours) ?? null;
}