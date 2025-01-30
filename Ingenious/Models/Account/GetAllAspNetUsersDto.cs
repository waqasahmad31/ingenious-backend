using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ingenious.Models.Account
{
    public class GetAllAspNetUsersDto
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public List<AspNetUsersDto> Users { get; set; }
    }
    public class AspNetUsersDto
    {
        public string AspNetUserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsDeleted { get; set; }

        public int AddressId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
