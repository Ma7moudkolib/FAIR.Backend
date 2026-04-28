using Microsoft.AspNetCore.Identity;
namespace FAIR.Domain.Entities.Identity
{
    public abstract class AppUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
