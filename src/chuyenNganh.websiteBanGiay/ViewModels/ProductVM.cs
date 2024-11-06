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

    }
}
