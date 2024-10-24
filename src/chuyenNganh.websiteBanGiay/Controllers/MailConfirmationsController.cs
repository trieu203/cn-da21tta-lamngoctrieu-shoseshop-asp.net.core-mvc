using chuyenNganh.websiteBanGiay.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace chuyenNganh.websiteBanGiay.Controllers
{
    public class MailConfirmationsController : Controller
    {
        private readonly ChuyenNganhContext _context;

        public MailConfirmationsController(ChuyenNganhContext context)
        {
            _context = context;
        }

        // GET: MailConfirmations
        public async Task<IActionResult> Index()
        {
            var chuyenNganhContext = _context.MailConfirmations.Include(m => m.Order);
            return View(await chuyenNganhContext.ToListAsync());
        }

        // GET: MailConfirmations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mailConfirmation = await _context.MailConfirmations
                .Include(m => m.Order)
                .FirstOrDefaultAsync(m => m.MailId == id);
            if (mailConfirmation == null)
            {
                return NotFound();
            }

            return View(mailConfirmation);
        }

        // GET: MailConfirmations/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId");
            return View();
        }

        // POST: MailConfirmations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MailId,OrderId,EmailSent,SentDate")] MailConfirmation mailConfirmation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mailConfirmation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", mailConfirmation.OrderId);
            return View(mailConfirmation);
        }

        // GET: MailConfirmations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mailConfirmation = await _context.MailConfirmations.FindAsync(id);
            if (mailConfirmation == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", mailConfirmation.OrderId);
            return View(mailConfirmation);
        }

        // POST: MailConfirmations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MailId,OrderId,EmailSent,SentDate")] MailConfirmation mailConfirmation)
        {
            if (id != mailConfirmation.MailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mailConfirmation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MailConfirmationExists(mailConfirmation.MailId))
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
            ViewData["OrderId"] = new SelectList(_context.Orders, "OrderId", "OrderId", mailConfirmation.OrderId);
            return View(mailConfirmation);
        }

        // GET: MailConfirmations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mailConfirmation = await _context.MailConfirmations
                .Include(m => m.Order)
                .FirstOrDefaultAsync(m => m.MailId == id);
            if (mailConfirmation == null)
            {
                return NotFound();
            }

            return View(mailConfirmation);
        }

        // POST: MailConfirmations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mailConfirmation = await _context.MailConfirmations.FindAsync(id);
            if (mailConfirmation != null)
            {
                _context.MailConfirmations.Remove(mailConfirmation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MailConfirmationExists(int id)
        {
            return _context.MailConfirmations.Any(e => e.MailId == id);
        }
    }
}
