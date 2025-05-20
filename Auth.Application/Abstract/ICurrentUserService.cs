using Auth.Application.Common.Responses;

namespace Auth.Application.Abstract;

public interface ICurrentUserService
{
    Result<string> GetCurrentUserEmail();
    Result<bool> IsAuthenticated();
}