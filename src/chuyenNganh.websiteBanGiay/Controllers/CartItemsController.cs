using chuyenNganh.websiteBanGiay.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace chuyenNganh.websiteBanGiay.Controllers
{
    public class CartItemsController : Controller
    {
        private readonly ChuyenNganhContext _context;

        public CartItemsController(ChuyenNganhContext context)
        {
            _context = context;
        }

        // GET: CartItems
        public async Task<IActionResult> Index()
        {
            var chuyenNganhContext = _context.CartItems.Include(c => c.Cart).Include(c => c.Product);
            return View(await chuyenNganhContext.ToListAsync());
        }

        // GET: CartItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems
                .Include(c => c.Cart)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.CartItemId == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // GET: CartItems/Create
        public IActionResult Create()
        {
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId");
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            return View();
        }

        // POST: CartItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartItemId,CartId,ProductId,Size,Quantity,PriceAtTime")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", cartItem.CartId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", cartItem.ProductId);
            return View(cartItem);
        }

        // GET: CartItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", cartItem.CartId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", cartItem.ProductId);
            return View(cartItem);
        }

        // POST: CartItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartItemId,CartId,ProductId,Size,Quantity,PriceAtTime")] CartItem cartItem)
        {
            if (id != cartItem.CartItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cartItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartItemExists(cartItem.CartItemId))
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
            ViewData["CartId"] = new SelectList(_context.Carts, "CartId", "CartId", cartItem.CartId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", cartItem.ProductId);
            return View(cartItem);
        }

        // GET: CartItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItems
                .Include(c => c.Cart)
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.CartItemId == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // POST: CartItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartItemExists(int id)
        {
            return _context.CartItems.Any(e => e.CartItemId == id);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Action))
            {
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            // Tìm CartItem theo CartItemId
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart) // Bao gồm thông tin Cart
                .FirstOrDefaultAsync(ci => ci.CartItemId == request.Id);

            if (cartItem == null || cartItem.Cart == null || !cartItem.Cart.IsActive)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
            }

            // Cập nhật số lượng dựa trên action
            if (request.Action.Equals("plus", StringComparison.OrdinalIgnoreCase))
            {
                cartItem.Quantity++;
            }
            else if (request.Action.Equals("minus", StringComparison.OrdinalIgnoreCase) && cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
            }
            else
            {
                return Json(new { success = false, message = "Hành động không hợp lệ." });
            }

            // Lưu thay đổi vào database
            try
            {
                _context.Update(cartItem);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi cập nhật cơ sở dữ liệu: {ex.Message}" });
            }

            // Tính tổng tiền giỏ hàng
            var cartTotal = await _context.CartItems
                .Where(ci => ci.CartId == cartItem.CartId)
                .SumAsync(ci => ci.Quantity * ci.PriceAtTime);

            return Json(new
            {
                success = true,
                quantity = cartItem.Quantity,
                totalPriceFormatted = string.Format("{0:N0} VND", cartItem.Quantity * cartItem.PriceAtTime),
                cartTotalFormatted = string.Format("{0:N0} VND", cartTotal + 50000) // Thêm phí vận chuyển
            });
        }

        public class UpdateQuantityRequest
        {
            public int Id { get; set; }
            public string? Action { get; set; }
        }
    }
}
