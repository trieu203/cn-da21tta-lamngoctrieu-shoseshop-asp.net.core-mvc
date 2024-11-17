using chuyenNganh.websiteBanGiay.Data;
using chuyenNganh.websiteBanGiay.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace chuyenNganh.websiteBanGiay.Controllers
{
    public class UsersController : Controller
    {
        private readonly ChuyenNganhContext _context;

        public UsersController(ChuyenNganhContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        public async Task<IActionResult> DangKy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                string defaultRole = "User";
                model.Role ??= defaultRole;
                var user = new User
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    Email = model.Email,
                    FullName = model.FullName,
                    SDT = model.SDT,
                    Address = model.Address,
                    ImageUrl = model.ImageUrl,
                    GioiTinh = model.GioiTinh,
                    NgaySinh = model.NgaySinh.HasValue
                        ? model.NgaySinh.Value.ToDateTime(new TimeOnly(0, 0))
                        : (DateTime?)null,
                    CreatedDate = DateOnly.FromDateTime(DateTime.Now)
                };

                _context.Add(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đăng ký thành công!";
                return RedirectToAction("DangKy");
            }
            return View(model);
        }


        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,UserName,Password,Email,FullName,SDT,Address,ImageUrl,GioiTinh,NgaySinh,Role,CreatedDate")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,UserName,Password,Email,FullName,SDT,Address,ImageUrl,GioiTinh,NgaySinh,Role,CreatedDate")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
