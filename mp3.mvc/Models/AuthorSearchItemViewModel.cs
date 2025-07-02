namespace mp3.mvc.Models
{
    public class AuthorSearchItemViewModel
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }
        public int Views { get; set; }
        public int NumberOfTracks { get; set; }
    }
}
