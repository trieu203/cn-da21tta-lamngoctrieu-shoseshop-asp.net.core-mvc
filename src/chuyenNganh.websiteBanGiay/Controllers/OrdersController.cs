using chuyenNganh.websiteBanGiay.Data;
using chuyenNganh.websiteBanGiay.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace chuyenNganh.websiteBanGiay.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ChuyenNganhContext _context;

        public OrdersController(ChuyenNganhContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var chuyenNganhContext = _context.Orders.Include(o => o.User);
            return View(await chuyenNganhContext.ToListAsync());
        }

        //Lịch sử mua hàng
        public IActionResult History()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Dangnhap", "Users");
            }

            // Lấy danh sách đơn hàng của người dùng
            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == int.Parse(userId))
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            // Tạo ViewModel
            var model = orders.Select(o => new OrderHistoryViewModel
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate.HasValue ? o.OrderDate.Value.ToString("dd/MM/yyyy HH:mm") : "N/A",
                TotalAmount = o.TotalAmount,
                OrderStatus = o.OrderStatus ?? "N/A",
                OrderItems = o.OrderItems.Select(oi => new OrderItemViewModel
                {
                    ProductName = oi.Product?.ProductName ?? "Sản phẩm không tồn tại",
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    Size = oi.Size
                }).ToList()
            }).ToList();

            return View(model);
        }


        //Hủy đơn hàng
        [HttpPost]
        public IActionResult CancelOrder(int orderId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Dangnhap", "Users");
            }

            // Tìm đơn hàng dựa trên orderId và userId
            var order = _context.Orders
                .Include(o => o.OrderItems) // Bao gồm các sản phẩm trong đơn hàng
                .FirstOrDefault(o => o.OrderId == orderId && o.UserId == int.Parse(userId));

            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng.";
                return RedirectToAction("History");
            }

            if (order.OrderStatus != "Pending")
            {
                TempData["ErrorMessage"] = "Chỉ có thể hủy đơn hàng đang chờ xử lý.";
                return RedirectToAction("History");
            }

            // Hoàn trả số lượng sản phẩm về kho
            foreach (var item in order.OrderItems)
            {
                // Tìm sản phẩm theo ProductId và Size
                var productSize = _context.ProductSizes
                    .FirstOrDefault(ps => ps.ProductId == item.ProductId && ps.Size == item.Size);

                if (productSize != null)
                {
                    productSize.Quantity += item.Quantity; // Hoàn trả số lượng
                    _context.ProductSizes.Update(productSize);
                }
            }

            // Cập nhật trạng thái đơn hàng thành "Cancelled"
            order.OrderStatus = "Cancelled";
            _context.Orders.Update(order);

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công và số lượng sản phẩm đã được hoàn trả.";
            return RedirectToAction("History");
        }




        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,UserId,OrderDate,TotalAmount,OrderStatus,ShippingAddress,Email,PhoneNumber")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", order.UserId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,UserId,OrderDate,TotalAmount,OrderStatus,ShippingAddress,Email,PhoneNumber")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
