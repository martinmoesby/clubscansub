using ClubScansub.Utility.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace ClubScansub.Utility.Managers
{
    public interface IAuthService
    {
        string SecretKey { get; set; }
        bool IsTokenValid(string token);
        string GenerateToken(IAuthContainerModel model);
        IEnumerable<Claim> GetTokenClaims(string token);
    }
}
