using Microsoft.AspNetCore.Identity;

namespace Authentication.Domain.Entities
{
    public class Appuser : IdentityUser
    {
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
