using mp3.mvc.Enums;

namespace mp3.mvc.Models
{
    public class NotificationViewModel
    {
        public NotificationType type { get; set; } = 0;
        public string message { get; set; } = "";
    }
}
