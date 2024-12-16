using chuyenNganh.websiteBanGiay.Data;
using chuyenNganh.websiteBanGiay.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;


namespace chuyenNganh.websiteBanGiay.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ChuyenNganhContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ChuyenNganhContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }


        //Index
        public async Task<IActionResult> Index(int? Category, int page = 1, int pageSize = 9)
        {
            _logger.LogInformation("Truy cập danh sách sản phẩm. Category: {Category}, Page: {Page}, PageSize: {PageSize}", Category, page, pageSize);

            try
            {
                var productQuery = _context.Products.AsQueryable();

                if (Category.HasValue)
                {
                    _logger.LogInformation("Lọc sản phẩm theo danh mục {Category}", Category.Value);
                    productQuery = productQuery.Where(p => p.CategoryId == Category.Value);
                }

                int totalItems = await productQuery.CountAsync();
                _logger.LogInformation("Tổng số sản phẩm: {TotalItems}", totalItems);

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

                _logger.LogInformation("Lấy sản phẩm thành công. Số lượng: {ProductCount}", products.Count);

                var paginatedResult = new PaginatedList<ProductVM>(products, totalItems, page, pageSize);
                ViewBag.CurrentCategory = Category;

                return View(paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách sản phẩm.");
                return StatusCode(500, "Đã xảy ra lỗi, vui lòng thử lại sau.");
            }
        }


        //Search
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
                _logger.LogWarning("Không tìm thấy ID sản phẩm.");
                return NotFound();
            }

            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.ProductSizes)
                    .Include(p => p.Reviews)
                        .ThenInclude(r => r.User)
                    .Include(p => p.WishLists)
                    .Include(p => p.CartItems)
                    .SingleOrDefaultAsync(p => p.ProductId == id);

                if (product == null)
                {
                    _logger.LogWarning("Không tìm thấy sản phẩm với ID: {Id}", id);
                    return NotFound();
                }

                _logger.LogInformation("Lấy chi tiết sản phẩm thành công. ID: {Id}", id);

                ViewBag.QuantityAvailable = product.ProductSizes.Sum(ps => ps.Quantity);
                var reviews = product.Reviews
                    .Where(r => r.ProductId == id)
                    .Select(r => new ReviewVM
                    {
                        Rating = r.Rating ?? 0,
                        Comment = r.Comment,
                        UserName = r.User?.UserName ?? "Ẩn danh",
                        Email = r.User?.Email ?? "Không có email",
                        ReviewDate = r.ReviewDate ?? DateTime.MinValue,
                        ImageUrl = r.User?.ImageUrl ?? "user_boy.jpg"
                    })
                    .ToList();

                var result = new ProductVMDT
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    Price = product.Price,
                    Discount = product.Discount,
                    ImageUrl = product.ImageUrl,
                    Reviews = reviews,
                    AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0
                };

                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy chi tiết sản phẩm với ID: {Id}", id);
                return StatusCode(500, "Đã xảy ra lỗi, vui lòng thử lại sau.");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            _logger.LogInformation("Bắt đầu thêm đánh giá cho sản phẩm ID: {ProductId}", productId);

            if (!User.Identity.IsAuthenticated)
            {
                _logger.LogWarning("Người dùng chưa đăng nhập khi gửi đánh giá.");
                return RedirectToAction("Dangnhap", "Users");
            }

            // Kiểm tra tất cả các claims
            var allClaims = User.Claims.ToList();
            foreach (var claim in allClaims)
            {
                _logger.LogInformation("Claim: {Type} - {Value}", claim.Type, claim.Value);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Không tìm thấy UserId từ Claim.");
                return RedirectToAction("Dangnhap", "Users");
            }


            //var userId = userIdClaim.Value;

            // Kiểm tra tính hợp lệ của rating và comment
            if (rating < 1 || rating > 5 || string.IsNullOrWhiteSpace(comment))
            {
                _logger.LogWarning("Dữ liệu đánh giá không hợp lệ. Rating: {Rating}, Comment: {Comment}", rating, comment);
                ModelState.AddModelError("", "Vui lòng chọn số sao và nhập đánh giá.");
                return RedirectToAction("Details", "Products", new { id = productId });
            }

            try
            {
                // Tạo đối tượng Review mới
                var review = new Review
                {
                    ProductId = productId,
                    UserId = int.Parse(userId),
                    Rating = rating,
                    Comment = comment,
                    ReviewDate = DateTime.Now
                };

                // Tìm sản phẩm theo ProductId
                var product = await _context.Products
                    .Include(p => p.Reviews)
                    .SingleOrDefaultAsync(p => p.ProductId == productId);

                if (product == null)
                {
                    _logger.LogWarning("Sản phẩm không tồn tại với ID: {ProductId}", productId);
                    return NotFound();
                }

                // Thêm Review vào sản phẩm
                product.Reviews.Add(review);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _context.SaveChangesAsync();

                _logger.LogInformation("Thêm đánh giá thành công. Product ID: {ProductId}, User ID: {UserId}", productId, userId);

                TempData["SuccessMessage"] = "Đánh giá của bạn đã được gửi!";
                return RedirectToAction("Details", "Products", new { id = productId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm đánh giá cho sản phẩm ID: {ProductId}", productId);
                return StatusCode(500, "Đã xảy ra lỗi, vui lòng thử lại sau.");
            }
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
