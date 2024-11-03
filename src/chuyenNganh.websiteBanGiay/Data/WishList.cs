namespace chuyenNganh.websiteBanGiay.Data;

public partial class WishList
{
    public int WishListId { get; set; }

    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public DateTime? AddedDate { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
