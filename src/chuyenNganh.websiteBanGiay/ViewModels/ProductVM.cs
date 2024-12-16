using chuyenNganh.websiteBanGiay.Data;

namespace chuyenNganh.websiteBanGiay.ViewModels
{
    public class ProductVM
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public string? ImageUrl { get; set; }
        public int WishListId { get; set; }
        public int CartId { get; set; }
        public int? Rating { get; set; }
        public string? Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CategoryName { get; set; }
    }

    public class ProductVMDT
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public string? ImageUrl { get; set; }
        public string? Size { get; set; }
        public int Quantity { get; set; }
        public int WishListId { get; set; }
        public int CartId { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public double AverageRating { get; set; } 
        public List<ReviewVM>? Reviews { get; set; }

    }

    public class ReviewVM
    {
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public int? ProductId { get; set; }
        public string? ImageUrl { get; set; }
        public string UserName { get; set; } = "Ẩn danh";
        public string Email { get; set; } = "Không có email";
        public DateTime ReviewDate { get; set; }
    }

    public class ProductDetailsViewModel
    {
        public Product? Product { get; set; }
        public List<Review>? Reviews { get; set; }
        public double AverageRating { get; set; }
        public int UserRating { get; set; }
        public Review? NewReview { get; set; }
    }

}
