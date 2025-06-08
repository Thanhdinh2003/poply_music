namespace mp3.mvc.Models
{
    public class MediaAddViewModel
    {
        public string? Name { get; set; } = "";
        public string? Description { get; set; } = "";
        public Guid CategoryId { get; set; }
        public Guid AuthorId { get; set; }
        public IFormFile ContentFile { get; set; }
        public List<IFormFile> Avatar { get; set; }
    }
}
