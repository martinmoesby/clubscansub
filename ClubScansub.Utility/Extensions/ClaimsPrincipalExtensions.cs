using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClubScansub.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static string GetIdentityId(this ClaimsPrincipal user)
        {
            var claimsIdentity = (ClaimsIdentity)user.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                return null;

            return claim.Value;

        }
    }
}
