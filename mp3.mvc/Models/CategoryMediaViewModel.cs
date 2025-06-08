using App.Domain.Entities;

namespace mp3.mvc.Models
{
    public class CategoryMediaViewModel
    {
        public Category Category { get; set; }
        public List<Media> Medias { get; set; } = new List<Media>();
    }
}
