using chuyenNganh.websiteBanGiay.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using chuyenNganh.websiteBanGiay.Data;
using chuyenNganh.websiteBanGiay.ViewModels;

namespace chuyenNganh.websiteBanGiay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ChuyenNganhContext _context;

        public HomeController(ChuyenNganhContext context)
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
