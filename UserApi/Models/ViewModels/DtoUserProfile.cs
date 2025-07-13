namespace UserApi.Models.ViewModels
{
    public class DtoUserProfile
    {
        public Guid UserId { get; set; }
        public string? NickName { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
    }
}
