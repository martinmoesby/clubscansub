using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace ClubScansub.Utility.Models
{
    public class JWTContainerModel : IAuthContainerModel
    {

        public JWTContainerModel()
        {

        }

        public string SecretKey { get; set; } = "B49EE7BF&E5B4$4FA3£BF09@6A3DF65295D0";
        public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;
        public int ExpireMinutes { get; set; } = 10080;
        public Claim[] Claims { get; set; }
    }
}
