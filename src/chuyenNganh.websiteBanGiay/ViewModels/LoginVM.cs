using System.ComponentModel.DataAnnotations;

namespace chuyenNganh.websiteBanGiay.ViewModels
{
    public class LoginVM
    {
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Tên người dùng là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Tên người dùng tối đa 50 ký tự")]
        public string? UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MaxLength(20, ErrorMessage = "Mật khẩu tối đa 20 ký tự")]
        public string? Password { get; set; }
    }
}
