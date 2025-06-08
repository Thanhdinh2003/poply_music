using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities
{
    [Table("authors")]
    public class Author
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public Guid Id { get; set; }
        public string? AvatarUrl { get; set; } = "/images/authors/empty.jpg";
        public string? Name { get; set; }
        public List<Media> Media { get; set; } = new List<Media>();
    }
}
