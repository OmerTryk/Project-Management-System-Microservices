namespace ProjectApi.Models.ViewModels
{
    public class DtoProjectUpdate
    {
        public Guid ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? ProjectDescription { get; set; }
    }
}
