namespace ParkingLot;

public record HourPrice(decimal FirstHourPrice, decimal OtherHourPrice)
{
    public Guid Id { get; set; } = Guid.Empty;
    
    public decimal CalcTotal(int hours)
    {
        return hours == 1 ? FirstHourPrice : FirstHourPrice + (hours - 1) * OtherHourPrice;
    }
}