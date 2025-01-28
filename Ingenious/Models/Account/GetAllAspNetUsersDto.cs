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
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleId { get; set; }

        public bool SendSystemNotification { get; set; }
        public string UpdatedSystemNotificationBy { get; set; }

        public bool SendMaintenanceNotification { get; set; }
        public string UpdatedMaintenanceNotificationBy { get; set; }

        public bool SendAccountsNotification { get; set; }
        public string UpdatedAccountsNotificationBy { get; set; }

        public bool SendTransactionalNotification { get; set; }
        public string UpdatedTransactionalNotificationBy { get; set; }
    }
}
