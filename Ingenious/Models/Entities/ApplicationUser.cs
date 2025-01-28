using Microsoft.AspNetCore.Identity;

namespace Ingenious.Models.Entities
{
    public  class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public bool isBlocked { get; set; }

        public bool IsDeleted { get; set; }
    }
}
