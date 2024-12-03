namespace chuyenNganh.websiteBanGiay.ViewModels
{
    public class CartItem
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public int Discount { get; set; }
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtTime => Quantity * Price;
    }
}
