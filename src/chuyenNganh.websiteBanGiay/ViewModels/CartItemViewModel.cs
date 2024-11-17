namespace chuyenNganh.websiteBanGiay.ViewModels
{
    public class CartItemViewModel
    {
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ImageUrl { get; set; }
        public string? Size { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal PriceAtTime { get; set; }
        public decimal TotalPrice => Quantity * Price;
    }
}
