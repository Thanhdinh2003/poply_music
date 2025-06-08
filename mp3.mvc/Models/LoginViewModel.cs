namespace mp3.mvc.Models
{
    public class LoginViewModel
    {
        public string username { get; set; } = "";
        public string password { get; set; } = "";
        public bool isRememeberMe { get; set; } = false;
        public string RequestPath { get; set; }
    }
}
