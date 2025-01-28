using System.ComponentModel.DataAnnotations.Schema;

namespace Ingenious.Models.Account
{
    public class RegisterUserDto
    {
        public string RoleId { get; set; }
        public string Username { get; set; }
        public string ContactNumber { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public bool isBlocked { get; set; }
        public bool IsDeleted { get; set; }
    }
}
