using System.Security.Claims;

namespace ParkingLot.Infra;

public interface ILoggedUserAccessor
{
    User Get();
}

public class LoggedUserAccessor : ILoggedUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LoggedUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public User Get()
    {
        var userId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        if (userId is null || userName is null)
        {
            throw new Exception("User is not logged in");
        }

        return new User(Guid.Parse(userId), userId);
    }
}