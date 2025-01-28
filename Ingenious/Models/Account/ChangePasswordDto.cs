namespace Ingenious.Models.Account
{
    public class ChangePasswordDto
    {
        public string User_Id { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
