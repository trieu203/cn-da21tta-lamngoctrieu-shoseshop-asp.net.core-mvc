using chuyenNganh.websiteBanGiay.Data;
using chuyenNganh.websiteBanGiay.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace chuyenNganh.websiteBanGiay.Models.Components
{
    public class CartViewComponent : ViewComponent
    {
        private readonly ChuyenNganhContext _context;

        public CartViewComponent(ChuyenNganhContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            // Lấy User ID từ Claims
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                // Nếu người dùng chưa đăng nhập, trả về giỏ hàng trống
                return View("CartPanel", new Cartmodel
                {
                    Quantity = 0,
                    Total = 0
                });
            }

            // Lấy giỏ hàng từ cơ sở dữ liệu
            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product) // Bao gồm thông tin sản phẩm
                .FirstOrDefault(c => c.UserId == int.Parse(userId) && c.IsActive);

            if (cart == null || !cart.CartItems.Any())
            {
                // Nếu không có giỏ hàng hoặc giỏ hàng trống
                return View("CartPanel", new Cartmodel
                {
                    Quantity = 0,
                    Total = 0
                });
            }

            // Tổng hợp thông tin giỏ hàng
            var cartModel = new Cartmodel
            {
                Quantity = cart.CartItems.Sum(ci => ci.Quantity)
            };

            return View("CartPanel", cartModel);
        }
    }
}
