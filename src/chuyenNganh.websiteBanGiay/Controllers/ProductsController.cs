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


        public async Task<IActionResult> Index(int? Category, int page = 1, int pageSize = 9)
        {
            var productQuery = _context.Products.AsQueryable();

            if (Category.HasValue)
            {
                productQuery = productQuery.Where(p => p.CategoryId == Category.Value);
            }

            int totalItems = await productQuery.CountAsync();

            var products = await productQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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
                                    .FirstOrDefault(),
                    CartId = _context.Carts
                                    .Where(c => c.CartItems.Any(ci => ci.ProductId == p.ProductId))
                                    .Select(c => c.CartId)
                                    .FirstOrDefault(),
                    Rating = _context.Reviews
                                    .Where(r => r.ProductId == p.ProductId)
                                    .Select(r => r.Rating)
                                    .FirstOrDefault()
                })
                .ToListAsync();

            var paginatedResult = new PaginatedList<ProductVM>(products, totalItems, page, pageSize);

            ViewBag.CurrentCategory = Category;

            return View(paginatedResult);
        }


        public async Task<IActionResult> Search(string? query, int page = 1, int pageSize = 9)
        {
            // Khởi tạo truy vấn cơ bản
            var productQuery = _context.Products.AsQueryable();

            // Nếu có từ khóa tìm kiếm, áp dụng lọc theo tên sản phẩm
            if (!string.IsNullOrWhiteSpace(query))
            {
                productQuery = productQuery.Where(p => p.ProductName.Contains(query));
            }

            // Tính toán tổng số sản phẩm và phân trang
            int totalItems = await productQuery.CountAsync();

            var products = await productQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    Discount = p.Discount,
                    ImageUrl = p.ImageUrl
                })
                .ToListAsync();

            // Tạo đối tượng PaginatedList chứa sản phẩm đã phân trang
            var paginatedResult = new PaginatedList<ProductVM>(products, totalItems, page, pageSize);

            // Truyền đối tượng PaginatedList vào View
            return View(paginatedResult);
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

            // Tính tổng số lượng tồn kho từ ProductSizes
            ViewBag.QuantityAvailable = p.ProductSizes.Sum(ps => ps.Quantity);


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
