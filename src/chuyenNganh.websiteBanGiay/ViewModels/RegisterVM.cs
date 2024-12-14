using System.ComponentModel.DataAnnotations;

namespace chuyenNganh.websiteBanGiay.ViewModels
{
    public class RegisterVM
    {
        public int UserId { get; set; }

        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "Tên người dùng là bắt buộc")]
        [MaxLength(50, ErrorMessage = "Tên người dùng tối đa 50 ký tự")]
        public string? UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MaxLength(20, ErrorMessage = "Mật khẩu tối đa 20 ký tự")]
        public string? Password { get; set; }

        [Display(Name = "Xác nhận mật khẩu")]
        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp")]
        public string? ConfirmPassword { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string? Email { get; set; }

        [Display(Name = "Họ tên")]
        [MaxLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        public string? FullName { get; set; }

        [Display(Name = "Số điện thoại")]
        [MaxLength(10, ErrorMessage = "Số điện thoại tối đa 10 ký tự")]
        [RegularExpression(@"^0(3[2-9]|5[6|8|9]|7[0|6|7|8|9]|8[1-5|8]|9[0|1|3|4|7|8|9])\d{7}$", ErrorMessage = "Chưa đúng định dạng số điện thoại Việt Nam")]
        public string? SDT { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [MaxLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự")]
        public string? Address { get; set; }

        [MaxLength(255, ErrorMessage = "Đường dẫn hình ảnh tối đa 255 ký tự")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Giới tính")]
        [Required(ErrorMessage = "Giới tính là bắt buộc")]
        public bool GioiTinh { get; set; } = true;

        [Display(Name = "Ngày sinh")]
        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        [DataType(DataType.Date, ErrorMessage = "Định dạng ngày sinh không hợp lệ")]
        public DateOnly? NgaySinh { get; set; }

        [MaxLength(50, ErrorMessage = "Vai trò tối đa 50 ký tự")]
        public string? Role { get; set; } = "User";

        // Thuộc tính để chứa hình ảnh khi đăng ký
        [Display(Name = "Ảnh đại diện")]
        public IFormFile? Image { get; set; }

    }
}
