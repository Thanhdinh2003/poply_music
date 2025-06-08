using App.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities
{
    [Table("media")]
    public class Media : BaseEntity
    {
        public string? Name { get; set; }
        public string? ContentUrl { get; set; }
        public string? Description { get; set; }
        public int Type { get; set; }
        public decimal Price { get; set; } = 0;
        public bool IsHidden { get; set; } = false;
        public bool IsLocked { get; set; } = false;
        public Guid AuthorId { get; set; }
        public Author? Author { get; set; }
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public List<MediaContent> MediaContent { get; set; } = new List<MediaContent>();
        public List<MediaViewHistory> MediaViewHistory { get; set; } = new List<MediaViewHistory>();
        public List<FavouriteCollection> FavouriteCollections { get; set; } = new List<FavouriteCollection>();
    }
}
