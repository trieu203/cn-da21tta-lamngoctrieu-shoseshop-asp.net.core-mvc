namespace chuyenNganh.websiteBanGiay.Data;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public int? CartId { get; set; }

    public int? ProductId { get; set; }

    public string? Size { get; set; }

    public int Quantity { get; set; }

    public decimal PriceAtTime { get; set; }

    public string? ImageUrl { get; set; }

    public string? ProductName { get; set; }

    public virtual Cart? Cart { get; set; }

    public virtual Product? Product { get; set; }
}
