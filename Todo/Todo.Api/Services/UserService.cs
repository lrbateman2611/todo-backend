namespace Todo.Api.Services;

public interface IUserService
{
    string GetCurrentUserId();
}

public class UserService(IHttpContextAccessor httpContextAccessor) : IUserService
{
    public string GetCurrentUserId()
    {
        var userId = httpContextAccessor.HttpContext?.User.Identity?.Name;
        
        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException("User ID not found in claims");
        }

        return userId;
    }
}
