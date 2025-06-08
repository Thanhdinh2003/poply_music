using App.Common.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Domain.Entities
{
    [Table("users")]
    public class User : BaseEntity
    {
        public string? Username { get; set; }
        public string? DisplayName { get; set; }
        public string? AvatarUrl { get; set; }
        public int Gender { get; set; }
        public DateTime Dob { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool IsLocked { get; set; } = false;
        public bool IsAdmin { get; set; } = false;
        public string? Password { get; set; }

        public bool IsPremiumAccount { get; set; } = false;
    }
}
