using System.ComponentModel.DataAnnotations;

namespace chuyenNganh.websiteBanGiay.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [Display(Name = "Tên người dùng")]
        public string? UserName { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [Display(Name = "Mật khẩu mới")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        public string? NewPassword { get; set; }

        [Required]
        [Display(Name = "Xác nhận mật khẩu mới")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string? ConfirmPassword { get; set; }

        public string? Token { get; set; }
        public string? UserName { get; set; }
    }

}
