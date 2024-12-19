namespace chuyenNganh.websiteBanGiay.ViewModels
{
    public class UserEditVM
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public string? Sdt { get; set; }
        public string? Address { get; set; }
        public bool? GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? ImageUrl { get; set; }
        public IFormFile? Image { get; set; }
    }

}
