using chuyenNganh.websiteBanGiay.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using chuyenNganh.websiteBanGiay.Helpers;

namespace chuyenNganh.websiteBanGiay.Controllers
{
    public class CartsController : Controller
    {
        private readonly ChuyenNganhContext _context;

        public CartsController(ChuyenNganhContext context)
        {
            _context = context;
        }

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>
            (Constand.Cart_Key) ?? new List<CartItem> ();


        // GET: Carts
        public IActionResult Index()
        {
            // Lấy giỏ hàng từ session hoặc tạo mới nếu chưa có
            var gioHang = HttpContext.Session.Get<List<CartItem>>(Constand.Cart_Key) ?? new List<CartItem>();

            // Truyền danh sách CartItem trực tiếp vào View
            return View(gioHang);
        }


        // Kiểm tra và thêm sản phẩm vào giỏ hàng
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var productSize = await _context.ProductSizes
                .Include(ps => ps.Product)
                .FirstOrDefaultAsync(ps => ps.ProductId == id);

            if (productSize == null || quantity <= 0)
            {
                TempData["Message"] = "Sản phẩm không hợp lệ hoặc số lượng không hợp lệ.";
                return RedirectToAction("Index", "Products");
            }

            if (quantity > productSize.Quantity)
            {
                TempData["Message"] = $"Số lượng yêu cầu vượt quá số lượng tồn kho. Chỉ còn {productSize.Quantity} sản phẩm.";
                return RedirectToAction("Index", "Products");
            }

            var gioHang = HttpContext.Session.Get<List<CartItem>>(Constand.Cart_Key) ?? new List<CartItem>();
            var item = gioHang.SingleOrDefault(p => p.ProductId == id);

            if (item == null)
            {
                item = new CartItem
                {
                    CartItemId = gioHang.Count == 0 ? 1 : gioHang.Max(ci => ci.CartItemId) + 1,
                    ProductId = productSize.ProductId,
                    ProductName = productSize.Product.ProductName,
                    Quantity = quantity,
                    PriceAtTime = productSize.Product.Price,
                    ImageUrl = productSize.Product.ImageUrl,
                    Size = productSize.Size
                };
                gioHang.Add(item);
            }
            else
            {
                item.Quantity += quantity;
            }

            HttpContext.Session.Set(Constand.Cart_Key, gioHang);
            TempData["Message"] = "Sản phẩm đã được thêm vào giỏ hàng.";
            return RedirectToAction("Index");
        }



        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            var gioHang = HttpContext.Session.Get<List<CartItem>>(Constand.Cart_Key) ?? new List<CartItem>();
            var itemToRemove = gioHang.SingleOrDefault(x => x.CartItemId == id);

            if (itemToRemove != null)
            {
                gioHang.Remove(itemToRemove);
                HttpContext.Session.Set(Constand.Cart_Key, gioHang);
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
