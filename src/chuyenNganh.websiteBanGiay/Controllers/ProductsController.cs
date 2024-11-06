using chuyenNganh.websiteBanGiay.Data;
using chuyenNganh.websiteBanGiay.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace chuyenNganh.websiteBanGiay.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ChuyenNganhContext _context;

        public ProductsController(ChuyenNganhContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(int? Category)
        {
            // Truy vấn cơ bản từ bảng Products
            var productQuery = _context.Products.AsQueryable();

            // Lọc theo Category nếu có
            if (Category.HasValue)
            {
                productQuery = productQuery.Where(p => p.CategoryId == Category.Value);
            }

            // Thực hiện truy vấn với Select để tạo ProductVM
            var result = await productQuery
                .Select(p => new ProductVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Discount = p.Discount,
                    ImageUrl = p.ImageUrl,
                    WishListId = _context.WishLists
                                    .Where(w => w.ProductId == p.ProductId)
                                    .Select(w => w.WishListId)
                                    .FirstOrDefault(), // Lấy WishListId đầu tiên liên kết với sản phẩm, hoặc 0 nếu không có
                    CartId = _context.Carts
                                    .Where(c => c.CartItems.Any(ci => ci.ProductId == p.ProductId))
                                    .Select(c => c.CartId)
                                    .FirstOrDefault(), // Lấy CartId đầu tiên có sản phẩm này trong giỏ hàng
                    Rating = _context.Reviews
                                    .Where(r => r.ProductId == p.ProductId)
                                    .Select(r => r.Rating)
                                    .FirstOrDefault() // Lấy đánh giá đầu tiên cho sản phẩm, hoặc null nếu không có
                })
                .ToListAsync();

            return View(result);
        }


        public async Task<IActionResult> Search(string? query)
        {
            // Truy vấn cơ bản từ bảng Products
            var productQuery = _context.Products.AsQueryable();

            // Lọc theo Category nếu có
            if (query != null)
            {
                productQuery = productQuery.Where(p => p.ProductName.Contains(query));
            }

            // Thực hiện truy vấn với Select để tạo ProductVM
            var result = await productQuery
                .Select(p => new ProductVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Discount = p.Discount,
                    ImageUrl = p.ImageUrl,
                    WishListId = _context.WishLists
                                    .Where(w => w.ProductId == p.ProductId)
                                    .Select(w => w.WishListId)
                                    .FirstOrDefault(), // Lấy WishListId đầu tiên liên kết với sản phẩm, hoặc 0 nếu không có
                    CartId = _context.Carts
                                    .Where(c => c.CartItems.Any(ci => ci.ProductId == p.ProductId))
                                    .Select(c => c.CartId)
                                    .FirstOrDefault(), // Lấy CartId đầu tiên có sản phẩm này trong giỏ hàng
                    Rating = _context.Reviews
                                    .Where(r => r.ProductId == p.ProductId)
                                    .Select(r => r.Rating)
                                    .FirstOrDefault() // Lấy đánh giá đầu tiên cho sản phẩm, hoặc null nếu không có
                })
                .ToListAsync();

            return View(result);
        }


        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var p = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductSizes)
                .Include(p => p.Reviews)
                .Include(p => p.WishLists)
                .Include(p => p.CartItems)
                .SingleOrDefaultAsync(m => m.ProductId == id);

            if (p == null)
            {
                return NotFound();
            }

            var result = new ProductVMDT
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                Discount = p.Discount,
                ImageUrl = p.ImageUrl,
                Size = p.ProductSizes.FirstOrDefault()?.Size,
                Quantity = p.ProductSizes.FirstOrDefault()?.Quantity ?? 0,
                WishListId = p.WishLists.FirstOrDefault()?.WishListId ?? 0,
                CartId = p.CartItems.FirstOrDefault()?.CartId ?? 0,
                Rating = p.Reviews.FirstOrDefault()?.Rating,
                Comment = p.Reviews.FirstOrDefault()?.Comment,
                UserName = p.Reviews.FirstOrDefault()?.User?.UserName,
                Email = p.Reviews.FirstOrDefault()?.User?.Email,
            };

            return View(result);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Price,Description,Discount,CategoryId,CreatedDate,ImageUrl")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,Price,Description,Discount,CategoryId,CreatedDate,ImageUrl")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
