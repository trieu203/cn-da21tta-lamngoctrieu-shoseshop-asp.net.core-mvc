using AutoMapper;
using chuyenNganh.websiteBanGiay.Data;
using chuyenNganh.websiteBanGiay.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;

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
                if (string.IsNullOrWhiteSpace(model.SDT))
                {
                    ModelState.AddModelError(string.Empty, "Số điện thoại không được để trống.");
                    return View(model);
                }

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

                // Kiểm tra định dạng email
                var emailRegex = @"^[a-zA-Z0-9._%+-]+@gmail\.com$";
                if (!Regex.IsMatch(model.Email, emailRegex))
                {
                    ModelState.AddModelError(string.Empty, "Địa chỉ email phải có đuôi @gmail.com.");
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
                    Sdt = model.SDT,
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
                    new Claim(ClaimTypes.Role, dbUser.Role),
                    new Claim("ImageUrl", dbUser.ImageUrl ?? "user_boy.jpg")
                };

                Console.WriteLine($"ImageUrl: {dbUser.ImageUrl}");

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                if (dbUser.Role == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }


        // GET: Users/Details/5
        public async Task<IActionResult> Details()
        {
            // Lấy UserName từ User.Identity.Name
            string userName = User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
            {
                // Nếu không tìm thấy tên người dùng, trả về lỗi 404
                return NotFound();
            }

            // Truy vấn User từ cơ sở dữ liệu dựa trên UserName
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName);  // Giả sử UserName là tên người dùng

            if (user == null)
            {
                // Nếu không tìm thấy người dùng, trả về lỗi 404
                return NotFound();
            }

            // Tạo ViewModel để hiển thị thông tin người dùng
            var userViewModel = new UserViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                ImageUrl = user.ImageUrl,
                SDT = user.Sdt,
                Address = user.Address,
                NgaySinh = user.NgaySinh,
                GioiTinh = user.GioiTinh,
                Role = user.Role
            };

            // Trả về View với dữ liệu của người dùng
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

            var userEditVM = new UserEditVM
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Sdt = user.Sdt,
                Address = user.Address,
                GioiTinh = user.GioiTinh,
                NgaySinh = user.NgaySinh,
                ImageUrl = user.ImageUrl,
                Role = user.Role
            };

            return View(userEditVM);
        }


        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserEditVM model)
        {
            if (id != model.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (model.NgaySinh.HasValue)
                {
                    var today = DateOnly.FromDateTime(DateTime.Today);
                    var birthDate = DateOnly.FromDateTime(model.NgaySinh.Value);

                    var age = today.Year - birthDate.Year;
                    if (birthDate > today.AddYears(-age))
                    {
                        age--;
                    }

                    if (age < 15)
                    {
                        ModelState.AddModelError(string.Empty, "Bạn phải ít nhất 15 tuổi để chỉnh sửa thông tin.");
                        return View(model);
                    }
                }

                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => (u.UserName == model.UserName || u.Email == model.Email) && u.UserId != model.UserId);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Tên người dùng hoặc email đã tồn tại.");
                    return View(model);
                }

                var emailRegex = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
                if (!Regex.IsMatch(model.Email, emailRegex))
                {
                    ModelState.AddModelError("Email", "Địa chỉ email không hợp lệ.");
                    return View(model);
                }

                string imageFileName = model.ImageUrl;

                if (model.Image != null)
                {
                    var fileExtension = Path.GetExtension(model.Image.FileName);
                    imageFileName = Guid.NewGuid().ToString() + fileExtension;

                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "users", imageFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(stream);
                    }
                }

                try
                {
                    var user = await _context.Users.FindAsync(id);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin người dùng
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.FullName = model.FullName;
                    user.Sdt = model.Sdt;
                    user.Address = model.Address;
                    user.GioiTinh = model.GioiTinh;
                    user.NgaySinh = model.NgaySinh;
                    user.ImageUrl = imageFileName;
                    user.Role = model.Role ?? "User";

                    // Nếu mật khẩu không được nhập mới, giữ mật khẩu cũ
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        user.Password = model.Password;
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(model.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction(nameof(Details));
            }

            return View(model);
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
