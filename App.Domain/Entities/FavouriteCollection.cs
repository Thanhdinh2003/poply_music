using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities
{
    [Table("favourite_collections")]
    public class FavouriteCollection
    {
        public Guid MediaId { get; set; }
        public Media? Media { get; set; }   
        public Guid UserId { get; set; }
        public User? User { get; set; }  
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
    }
}
