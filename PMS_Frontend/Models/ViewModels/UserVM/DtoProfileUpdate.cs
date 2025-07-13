namespace PMS_Frontend.Models.ViewModels.UserVM
{
    public class DtoProfileUpdate
    {
        public Guid UserId { get; set; }
        public string? NickName { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }
}
