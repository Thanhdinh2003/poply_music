using App.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities
{
    [Table("media_view_history")]
    public class MediaViewHistory : BaseEntity
    {
        public Guid MediaId { get; set; }
        public Media? Media { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
