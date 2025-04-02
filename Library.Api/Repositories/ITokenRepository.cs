using Microsoft.AspNetCore.Identity;

namespace Library.Api.Repositories
{
    public interface ITokenRepository
    {
        string createJWTToken(IdentityUser user, List<String> roles);
    }
}
