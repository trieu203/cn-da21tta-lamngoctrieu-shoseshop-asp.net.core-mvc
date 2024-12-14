﻿using AutoMapper;
using chuyenNganh.websiteBanGiay.Data;
using chuyenNganh.websiteBanGiay.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace chuyenNganh.websiteBanGiay.Controllers
{
    public class UsersController : Controller
    {
        private readonly ChuyenNganhContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsersController(ChuyenNganhContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách tất cả người dùng
            var users = await _context.Users.ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> DangKy()
        {
            return View();
        }

        public async Task<IActionResult> Dangnhap()
        {
            return View();
        }

        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Dangnhap", "Users");
        }

        //Đăng ký

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tuổi người dùng
                if (model.NgaySinh.HasValue)
                {
                    var today = DateOnly.FromDateTime(DateTime.Today); // Ngày hôm nay
                    var birthDate = model.NgaySinh.Value; // Ngày sinh từ model

                    // Tính toán tuổi
                    var age = today.Year - birthDate.Year;
                    if (birthDate > today.AddYears(-age))
                    {
                        age--;
                    }

                    // Kiểm tra nếu tuổi dưới 15
                    if (age < 15)
                    {
                        ModelState.AddModelError(string.Empty, "Bạn phải ít nhất 15 tuổi để đăng ký.");
                        return View(model);
                    }
                }

                // Kiểm tra xem người dùng đã tồn tại chưa
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == model.UserName || u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Tên người dùng hoặc email đã tồn tại.");
                    return View(model);
                }

                // Lưu tên file hình ảnh
                string imageFileName = null;

                if (model.Image != null)
                {
                    var fileExtension = Path.GetExtension(model.Image.FileName);
                    imageFileName = Guid.NewGuid().ToString() + fileExtension;

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img","users", imageFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(stream);
                    }
                }

                // Tạo người dùng mới
                var user = new User
                {
                    UserName = model.UserName,
                    Password = model.Password,
                    Email = model.Email,
                    FullName = model.FullName,
                    SDT = model.SDT,
                    Address = model.Address,
                    GioiTinh = model.GioiTinh,
                    Role = "User",
                    NgaySinh = model.NgaySinh?.ToDateTime(TimeOnly.MinValue),
                    CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                    ImageUrl = imageFileName // Lưu tên hình ảnh vào cơ sở dữ liệu
                };

                // Thêm người dùng vào cơ sở dữ liệu
                _context.Add(user);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng ký thành công!";
                return RedirectToAction("DangKy");
            }

            return View(model);
        }

        // Đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dangnhap(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<User>(model);

                var dbUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == user.UserName && u.Password == user.Password);

                if (dbUser == null)
                {
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, dbUser.UserId.ToString()),
                    new Claim(ClaimTypes.Name, dbUser.UserName),
                    new Claim(ClaimTypes.Role, dbUser.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Index", "Home");
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

            var userViewModel = new UserViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                SDT = user.SDT,
                Address = user.Address,
                NgaySinh = user.NgaySinh,
                GioiTinh = user.GioiTinh,
                Role = user.Role
            };

            return View(userViewModel);
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
