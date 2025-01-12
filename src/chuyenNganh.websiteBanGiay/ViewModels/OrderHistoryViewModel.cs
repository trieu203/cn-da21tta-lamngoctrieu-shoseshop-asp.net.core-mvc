namespace chuyenNganh.websiteBanGiay.ViewModels
{
    public class OrderHistoryViewModel
    {
        public int OrderId { get; set; }
        public string? OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? OrderStatus { get; set; }
        public string TranslatedOrderStatus
        {
            get
            {
                return OrderStatus switch
                {
                    "Pending" => "Đang chờ xử lý",
                    "Confirmed" => "Đã xác nhận",
                    "Shipped" => "Đã giao hàng",
                    "Cancelled" => "Đã hủy",
                    "Completed" => "Hoàn thành",
                    _ => "Không rõ trạng thái"
                };
            }
        }
        public List<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();
    }

    public class OrderItemViewModel
    {
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string? Size { get; set; }
    }
}
