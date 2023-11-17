namespace ParkingLot;

public interface IHourPriceProvider
{
    Task<HourPrice?> GetHourPrice(CancellationToken cancellationToken);
}