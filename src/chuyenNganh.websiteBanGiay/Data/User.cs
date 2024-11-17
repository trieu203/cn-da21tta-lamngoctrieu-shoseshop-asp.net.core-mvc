namespace chuyenNganh.websiteBanGiay.Data;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? FullName { get; set; }

    public string? SDT { get; set; }

    public string? Address { get; set; }

    public string? ImageUrl { get; set; }

    public bool GioiTinh { get; set; }

    public DateTime? NgaySinh { get; set; }

    public string? Role { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<WishList> WishLists { get; set; } = new List<WishList>();
}
