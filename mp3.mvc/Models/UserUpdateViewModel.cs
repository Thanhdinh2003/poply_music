namespace mp3.mvc.Models
{
    public class UserUpdateViewModel
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? DisplayName { get; set; }
        public IFormFile? Avatar { get; set; }
        public int Gender { get; set; }
        public DateTime Dob { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool IsLocked { get; set; }
        public string? Password { get; set; }
    }
}
