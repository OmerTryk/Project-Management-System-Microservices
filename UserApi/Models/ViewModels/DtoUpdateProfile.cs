namespace UserApi.Models.ViewModels
{
    public class DtoUpdateProfile
    {
        public Guid UserId { get; set; }
        public string? NickName { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
