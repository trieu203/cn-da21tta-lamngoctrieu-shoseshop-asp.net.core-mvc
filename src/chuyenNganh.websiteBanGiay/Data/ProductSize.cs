using System.ComponentModel.DataAnnotations;

namespace chuyenNganh.websiteBanGiay.Data;

public partial class ProductSize
{
    public int ProductSizeId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn sản phẩm.")]
    public int? ProductId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập kích cỡ.")]
    [StringLength(10, ErrorMessage = "Kích cỡ không được vượt quá 10 ký tự.")]
    public string Size { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập số lượng.")]
    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập giá.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0.")]
    public decimal PriceAtTime { get; set; }

    public virtual Product? Product { get; set; }
}
