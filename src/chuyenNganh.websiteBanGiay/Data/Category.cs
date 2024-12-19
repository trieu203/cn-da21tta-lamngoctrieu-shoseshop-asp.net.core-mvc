using System.ComponentModel.DataAnnotations;

namespace chuyenNganh.websiteBanGiay.Data;

public partial class Category
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Tên danh mục không được để trống.")]
    public string CategoryName { get; set; } = null!;

    [Required(ErrorMessage = "Mô tả không được để trống.")]
    public string? Description { get; set; }

    public DateOnly? CreatedDate { get; set; }
    public string? ImageUrl { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
