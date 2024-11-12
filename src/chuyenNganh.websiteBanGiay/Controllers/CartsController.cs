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

        const string Cart_Key = "MyCart";

        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>
            (Cart_Key) ?? new List<CartItem> ();


        // GET: Carts
        public IActionResult Index()
        {
            // Lấy giỏ hàng từ session hoặc tạo mới nếu chưa có
            var gioHang = HttpContext.Session.Get<List<CartItem>>(Cart_Key) ?? new List<CartItem>();

            // Truyền danh sách CartItem trực tiếp vào View
            return View(gioHang);
        }


        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            // Lấy giỏ hàng từ session hoặc tạo mới nếu chưa có
            var gioHang = HttpContext.Session.Get<List<CartItem>>(Cart_Key) ?? new List<CartItem>();

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var item = gioHang.SingleOrDefault(p => p.ProductId == id);

            if (item == null)
            {
                // Truy xuất sản phẩm từ database
                var product = await _context.Products.SingleOrDefaultAsync(p => p.ProductId == id);
                if (product == null)
                {
                    TempData["Message"] = $"Không tìm thấy sản phẩm có mã {id}";
                    return Redirect("/404");
                }

                // Tạo mới một CartItem và thêm vào giỏ hàng
                item = new CartItem
                {
                    CartItemId = gioHang.Count + 1,
                    ProductId = product.ProductId,
                    Quantity = quantity,
                    PriceAtTime = product.Price,
                    ImageUrl = product.ImageUrl,
                    Size = "Mặc định"
                };
                gioHang.Add(item);
            }
            else
            {
                // Nếu sản phẩm đã tồn tại, tăng số lượng
                item.Quantity += quantity;
            }

            // Cập nhật lại session
            HttpContext.Session.Set(Cart_Key, gioHang);

            // Chuyển hướng người dùng về trang giỏ hàng
            return RedirectToAction("Index");
        }
        



        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            var gioHang = HttpContext.Session.Get<List<CartItem>>(Cart_Key) ?? new List<CartItem>();
            var itemToRemove = gioHang.SingleOrDefault(x => x.CartItemId == id);

            if (itemToRemove != null)
            {
                gioHang.Remove(itemToRemove);
                HttpContext.Session.Set(Cart_Key, gioHang);
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
