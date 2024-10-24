using chuyenNganh.websiteBanGiay.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace chuyenNganh.websiteBanGiay.Controllers
{
    public class ProductSizesController : Controller
    {
        private readonly ChuyenNganhContext _context;

        public ProductSizesController(ChuyenNganhContext context)
        {
            _context = context;
        }

        // GET: ProductSizes
        public async Task<IActionResult> Index()
        {
            var chuyenNganhContext = _context.ProductSizes.Include(p => p.Product);
            return View(await chuyenNganhContext.ToListAsync());
        }

        // GET: ProductSizes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productSize = await _context.ProductSizes
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductSizeId == id);
            if (productSize == null)
            {
                return NotFound();
            }

            return View(productSize);
        }

        // GET: ProductSizes/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName");
            return View();
        }

        // POST: ProductSizes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductSizeId,ProductId,Size,Quantity,PriceAtTime")] ProductSize productSize)
        {
            // Kiểm tra xem size đã tồn tại cho sản phẩm cụ thể chưa
            var existingSize = await _context.ProductSizes
                .AnyAsync(ps => ps.ProductId == productSize.ProductId && ps.Size == productSize.Size);

            if (existingSize)
            {
                ModelState.AddModelError("Size", "Kích cỡ này đã tồn tại cho sản phẩm được chọn.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(productSize);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productSize.ProductId);
            return View(productSize);
        }


        // GET: ProductSizes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productSize = await _context.ProductSizes.FindAsync(id);
            if (productSize == null)
            {
                return NotFound();
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productSize.ProductId);
            return View(productSize);
        }

        // POST: ProductSizes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductSizeId,ProductId,Size,Quantity,PriceAtTime")] ProductSize productSize)
        {
            if (id != productSize.ProductSizeId)
            {
                return NotFound();
            }

            // Kiểm tra xem size đã tồn tại cho sản phẩm cụ thể chưa (ngoại trừ chính bản ghi hiện tại)
            var existingSize = await _context.ProductSizes
                .AnyAsync(ps => ps.ProductId == productSize.ProductId && ps.Size == productSize.Size && ps.ProductSizeId != productSize.ProductSizeId);

            if (existingSize)
            {
                ModelState.AddModelError("Size", "Kích cỡ này đã tồn tại cho sản phẩm được chọn.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productSize);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductSizeExists(productSize.ProductSizeId))
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

            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", productSize.ProductId);
            return View(productSize);
        }

        // GET: ProductSizes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productSize = await _context.ProductSizes
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductSizeId == id);
            if (productSize == null)
            {
                return NotFound();
            }

            return View(productSize);
        }

        // POST: ProductSizes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productSize = await _context.ProductSizes.FindAsync(id);
            if (productSize != null)
            {
                _context.ProductSizes.Remove(productSize);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductSizeExists(int id)
        {
            return _context.ProductSizes.Any(e => e.ProductSizeId == id);
        }
    }
}
