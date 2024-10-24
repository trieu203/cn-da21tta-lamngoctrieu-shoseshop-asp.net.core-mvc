using System;
using System.Collections.Generic;

namespace chuyenNganh.websiteBanGiay.Data;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public int Discount { get; set; }

    public int? CategoryId { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<WishList> WishLists { get; set; } = new List<WishList>();
}
