namespace ParkingLot.Infra;

public interface IUserService
{
    Task<bool> Authenticate(string username, string password, out User? user);
}

public class UserService : IUserService
{
    public Task<bool> Authenticate(string username, string password, out User? user)
    {
        if (username == "admin" && password == "admin")
        {
            user = new User(Guid.NewGuid(), username);
            return Task.FromResult(true);
        }

        user = null;
        return Task.FromResult(false);
    }
}