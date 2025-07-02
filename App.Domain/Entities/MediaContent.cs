using App.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities
{
    [Table("media_content")]
    public class MediaContent : BaseEntity
    {
        public int Type { get; set; }
        public string? Value { get; set; }
        public Guid  MediaId { get; set; }
    }
}
