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
}
