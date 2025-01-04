using chuyenNganh.websiteBanGiay.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace chuyenNganh.websiteBanGiay.Controllers
{
    public class WishListsController : Controller
    {
        private readonly ChuyenNganhContext _context;

        public WishListsController(ChuyenNganhContext context)
        {
            _context = context;
        }

        // GET: WishLists
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                // Nếu người dùng chưa đăng nhập
                return RedirectToAction("Dangnhap", "Users");
            }

            // Lấy danh sách yêu thích của người dùng hiện tại
            var wishListItems = await _context.WishLists
                .Where(w => w.UserId.ToString() == userId)
                .Include(w => w.Product)
                .ToListAsync();

            return View(wishListItems);
        }


        //AddToWishList
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToWishList(int productId)
        {
            // Lấy ID người dùng từ Claims trong Cookie Authentication
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Dangnhap", "Users");
            }

            // Kiểm tra xem sản phẩm đã có trong danh sách yêu thích chưa
            var existingWishList = await _context.WishLists
                .FirstOrDefaultAsync(w => w.UserId == int.Parse(userId) && w.ProductId == productId);

            // Nếu sản phẩm đã có trong danh sách yêu thích, thông báo lỗi
            if (existingWishList != null)
            {
                TempData["ErrorMessage"] = "Sản phẩm đã có trong danh sách yêu thích của bạn.";
                return RedirectToAction("Index", "WishLists", new { id = productId });
            }

            // Nếu sản phẩm chưa có trong danh sách yêu thích, thêm vào
            var wishList = new WishList
            {
                UserId = int.Parse(userId),
                ProductId = productId,
                AddedDate = DateTime.Now
            };

            // Thêm sản phẩm vào danh sách yêu thích trong cơ sở dữ liệu
            _context.WishLists.Add(wishList);
            await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu

            // Hiển thị thông báo thành công
            TempData["SuccessMessage"] = "Sản phẩm đã được thêm vào danh sách yêu thích!";
            return RedirectToAction("Index", "WishLists", new { id = productId });
        }


        //DeleteWishList
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            // Kiểm tra nếu người dùng đã đăng nhập
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dangnhap", "Users");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Dangnhap", "Users");
            }

            // Tìm sản phẩm trong danh sách yêu thích của người dùng
            var wishListItem = await _context.WishLists
                .FirstOrDefaultAsync(w => w.UserId == int.Parse(userId) && w.ProductId == productId);

            if (wishListItem != null)
            {
                _context.WishLists.Remove(wishListItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sản phẩm đã được xóa khỏi danh sách yêu thích.";
            }
            else
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại trong danh sách yêu thích.";
            }

            return RedirectToAction("Index");
        }


        // GET: WishLists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishList = await _context.WishLists
                .Include(w => w.Product)
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.WishListId == id);
            if (wishList == null)
            {
                return NotFound();
            }

            return View(wishList);
        }

        // GET: WishLists/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: WishLists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WishListId,UserId,ProductId,AddedDate")] WishList wishList)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wishList);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", wishList.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", wishList.UserId);
            return View(wishList);
        }

        // GET: WishLists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishList = await _context.WishLists.FindAsync(id);
            if (wishList == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", wishList.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", wishList.UserId);
            return View(wishList);
        }

        // POST: WishLists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WishListId,UserId,ProductId,AddedDate")] WishList wishList)
        {
            if (id != wishList.WishListId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wishList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WishListExists(wishList.WishListId))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductId", wishList.ProductId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", wishList.UserId);
            return View(wishList);
        }

        // GET: WishLists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var wishList = await _context.WishLists
                .Include(w => w.Product)
                .Include(w => w.User)
                .FirstOrDefaultAsync(m => m.WishListId == id);
            if (wishList == null)
            {
                return NotFound();
            }

            return View(wishList);
        }

        // POST: WishLists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wishList = await _context.WishLists.FindAsync(id);
            if (wishList != null)
            {
                _context.WishLists.Remove(wishList);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WishListExists(int id)
        {
            return _context.WishLists.Any(e => e.WishListId == id);
        }
    }
}
