using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace mp3.mvc.Models
{
    public class RegisterViewModel
    {
        [StringLength(20)]
        [DisplayName("Tên đăng nhập")]
        [Required(ErrorMessage = "{0} không được trống.")]
        public string username { get; set; } = "";
        [StringLength(50)]
        [DisplayName("Tên hiển thị")]
        [Required(ErrorMessage = "{0} không được trống.")]
        public string displayName { get; set; } = "";
        public int gender { get; set; } = 0;
        public DateTime dob { get; set; }
        [DisplayName("Địa chỉ")]
        [Required(ErrorMessage = "{0} không được trống")]
        [StringLength(200)]
        public string address { get; set; } = "";

        [DisplayName("Mật khẩu")]
        [Required(ErrorMessage = "{0} không được trống.")]
        [StringLength(200)]
        [MinLength(6, ErrorMessage = "{0} cần ít nhất 6 ký tự.")]
        public string? password { get; set; }
        [Required(ErrorMessage = "Nhập lại mật khẩu")]
        [StringLength(200)]
        [Compare("password", ErrorMessage = "Mật khẩu nhập lại không đúng. Vui lòng nhập lại!")]
        public string repeatedPassword { get; set; }
    }
}
