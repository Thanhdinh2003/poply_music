namespace mp3.mvc.Models
{
    public class AuthorUpdateViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }
        public IFormFile? Avatar { get; set; }
    }
}
