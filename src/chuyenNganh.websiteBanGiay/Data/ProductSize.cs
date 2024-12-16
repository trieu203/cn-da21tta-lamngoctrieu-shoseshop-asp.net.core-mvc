namespace chuyenNganh.websiteBanGiay.Data;

public partial class ProductSize
{
    public int ProductSizeId { get; set; }

    public int? ProductId { get; set; }

    public string Size { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal PriceAtTime { get; set; }

    public virtual Product? Product { get; set; }
}
