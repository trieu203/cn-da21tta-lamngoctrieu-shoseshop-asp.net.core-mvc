using System.ComponentModel.DataAnnotations;

namespace chuyenNganh.websiteBanGiay.ViewModels
{
    public class EmailPasswordLoginVM
    {
        [Display(Name = "Địa chỉ Email")]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string? Email { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MaxLength(20, ErrorMessage = "Mật khẩu tối đa 20 ký tự")]
        public string? Password { get; set; }
    }
    public class OTPVerificationVM
    {
        [Required]
        public string? Email { get; set; }

        [Display(Name = "Mã OTP")]
        [Required(ErrorMessage = "Mã OTP là bắt buộc")]
        [MaxLength(6, ErrorMessage = "OTP phải gồm 6 chữ số")]
        public string? OTP { get; set; }
    }

}
