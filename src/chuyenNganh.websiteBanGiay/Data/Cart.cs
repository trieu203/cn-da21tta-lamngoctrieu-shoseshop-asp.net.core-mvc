namespace chuyenNganh.websiteBanGiay.Data;

public partial class Cart
{
    public int CartId { get; set; }

    public int? UserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User? User { get; set; }
}
