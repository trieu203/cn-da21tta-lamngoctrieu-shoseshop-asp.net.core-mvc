using chuyenNganh.websiteBanGiay.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace chuyenNganh.websiteBanGiay.Controllers
{
    public class CartsController : Controller
    {
        private readonly ChuyenNganhContext _context;

        public CartsController(ChuyenNganhContext context)
        {
            _context = context;
        }


        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Dangnhap", "Users");
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == int.Parse(userId) && c.IsActive);

            if (cart == null || !cart.CartItems.Any())
            {
                ViewBag.Message = "Giỏ hàng của bạn đang trống.";
                return View(new List<CartItem>());
            }

            var cartItems = cart.CartItems.Select(ci => new
            {
                ci.ProductId,
                ci.ProductName,
                ci.Quantity,
                ci.PriceAtTime,
                ci.ImageUrl,
                ci.Size,
                TotalPrice = ci.Quantity * ci.PriceAtTime
            }).ToList();

            return View(cart.CartItems);
        }



        // Kiểm tra và thêm sản phẩm vào giỏ hàng
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            // Lấy ID người dùng từ Claims trong Cookie Authentication
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Dangnhap", "Users");
            }

            // Lấy thông tin sản phẩm và size
            var productSize = await _context.ProductSizes
                .Include(ps => ps.Product)
                .FirstOrDefaultAsync(ps => ps.ProductId == id);

            // Kiểm tra sản phẩm có tồn tại hay không
            if (productSize == null)
            {
                TempData["Message"] = "Sản phẩm không hợp lệ.";
                return RedirectToAction("Index", "Products");
            }

            // Kiểm tra số lượng hợp lệ
            if (quantity <= 0)
            {
                TempData["Message"] = "Số lượng không hợp lệ. Vui lòng nhập số lượng lớn hơn 0.";
                return RedirectToAction("Details", "Products", new { id });
            }

            if (quantity > 20)
            {
                TempData["Message"] = "Số lượng không hợp lệ. Vui lòng nhập số lượng ít hơn hoặc bằng 20.";
                return RedirectToAction("Details", "Products", new { id });
            }

            if (quantity > productSize.Quantity)
            {
                TempData["Message"] = $"Số lượng yêu cầu vượt quá số lượng tồn kho. Chỉ còn {productSize.Quantity} sản phẩm.";
                return RedirectToAction("Details", "Products", new { id });
            }

            // Kiểm tra giỏ hàng hiện tại của người dùng
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == int.Parse(userId) && c.IsActive);

            // Nếu người dùng chưa có giỏ hàng, tạo mới
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = int.Parse(userId),
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(); // Lưu để lấy `CartId`
            }

            // Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng chưa
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == id && ci.Size == productSize.Size);

            if (cartItem != null)
            {
                // Nếu sản phẩm đã tồn tại, cập nhật số lượng
                cartItem.Quantity += quantity;
                if (cartItem.Quantity > productSize.Quantity)
                {
                    TempData["Message"] = $"Không đủ hàng trong kho. Chỉ còn {productSize.Quantity} sản phẩm.";
                    return RedirectToAction("Details", "Products", new { id });
                }
            }
            else
            {
                // Nếu sản phẩm chưa tồn tại, thêm mới vào giỏ hàng
                cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = productSize.ProductId,
                    Quantity = quantity,
                    ProductName = productSize.Product.ProductName,
                    PriceAtTime = productSize.Product.Price,
                    Size = productSize.Size,
                    ImageUrl = productSize.Product.ImageUrl
                };
                _context.CartItems.Add(cartItem);
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            TempData["Message"] = "Sản phẩm đã được thêm vào giỏ hàng.";
            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            // Lấy ID người dùng từ Claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Dangnhap", "Users");
            }

            // Tìm CartItem trong cơ sở dữ liệu dựa trên `id` và `userId`
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemId == id && ci.Cart.UserId == int.Parse(userId) && ci.Cart.IsActive);

            if (cartItem != null)
            {
                // Xóa CartItem khỏi cơ sở dữ liệu
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
                TempData["SuccessMessage"] = "Sản phẩm đã được xóa khỏi giỏ hàng.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm trong giỏ hàng.";
            }

            return RedirectToAction("Index");
        }




        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CartId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartId,UserId,CreatedDate,IsActive")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", cart.UserId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", cart.UserId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartId,UserId,CreatedDate,IsActive")] Cart cart)
        {
            if (id != cart.CartId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.CartId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", cart.UserId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CartId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.CartId == id);
        }

    }
}
