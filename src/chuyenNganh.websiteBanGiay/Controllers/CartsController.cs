using chuyenNganh.websiteBanGiay.Data;
using chuyenNganh.websiteBanGiay.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Security.Claims;

namespace chuyenNganh.websiteBanGiay.Controllers
{
    public class CartsController : Controller
    {
        private readonly ChuyenNganhContext _context;

        public CartsController(ChuyenNganhContext context)
        {
            _context = context;
        }


        // GET: Carts
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Dangnhap", "Users");
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == int.Parse(userId) && c.IsActive);

            if (cart == null || !cart.CartItems.Any())
            {
                ViewBag.Message = "Giỏ hàng của bạn đang trống.";
                return View(new List<CartItem>());
            }

            var cartItems = cart.CartItems.Select(ci => new
            {
                ci.ProductId,
                ci.ProductName,
                ci.Quantity,
                ci.PriceAtTime,
                ci.ImageUrl,
                ci.Size,
                TotalPrice = ci.Quantity * ci.PriceAtTime
            }).ToList();

            return View(cart.CartItems);
        }


        // Kiểm tra và thêm sản phẩm vào giỏ hàng
        public async Task<IActionResult> AddToCart(int id, int quantity = 1, int size = 0)
        {
            // Lấy ID người dùng từ Claims trong Cookie Authentication
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Dangnhap", "Users");
            }

            var userIdInt = int.Parse(userId); // Chuyển đổi từ string sang int

            // Kiểm tra kích thước hợp lệ
            if (size == 0)
            {
                TempData["Message"] = "Vui lòng chọn kích thước sản phẩm.";
                return RedirectToAction("Details", "Products", new { id });
            }

            // Lấy thông tin sản phẩm và kích thước
            var productSize = await _context.ProductSizes
                .Include(ps => ps.Product)
                .FirstOrDefaultAsync(ps => ps.ProductId == id && ps.Size == size.ToString()); // Chuyển size sang string

            // Kiểm tra sản phẩm và kích thước có tồn tại không
            if (productSize == null)
            {
                TempData["Message"] = "Kích thước sản phẩm không hợp lệ.";
                return RedirectToAction("Details", "Products", new { id });
            }

            // Kiểm tra số lượng hợp lệ
            if (quantity <= 0)
            {
                TempData["Message"] = "Số lượng không hợp lệ. Vui lòng nhập số lượng lớn hơn 0.";
                return RedirectToAction("Details", "Products", new { id });
            }

            // Kiểm tra số lượng tồn kho
            if (quantity > productSize.Quantity)
            {
                TempData["Message"] = $"Số lượng yêu cầu vượt quá số lượng tồn kho. Chỉ còn {productSize.Quantity} sản phẩm.";
                return RedirectToAction("Details", "Products", new { id });
            }

            // Kiểm tra giỏ hàng hiện tại của người dùng
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userIdInt && c.IsActive);

            // Nếu người dùng chưa có giỏ hàng, tạo mới
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userIdInt,
                    CreatedDate = DateTime.Now,
                    IsActive = true
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(); // Lưu để lấy `CartId`
            }

            // Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng chưa
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == id && ci.Size == size.ToString()); // Chuyển size sang string

            if (cartItem != null)
            {
                // Nếu sản phẩm đã tồn tại, cập nhật số lượng
                var updatedQuantity = cartItem.Quantity + quantity;
                if (updatedQuantity > productSize.Quantity)
                {
                    TempData["Message"] = $"Không đủ hàng trong kho. Số lượng tối đa bạn có thể thêm là {productSize.Quantity - cartItem.Quantity}.";
                    return RedirectToAction("Details", "Products", new { id });
                }
                cartItem.Quantity = updatedQuantity;
            }
            else
            {
                // Nếu sản phẩm chưa tồn tại, thêm mới vào giỏ hàng
                cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = productSize.ProductId,
                    Quantity = quantity,
                    ProductName = productSize.Product.ProductName,
                    PriceAtTime = productSize.Product.Price,
                    Size = size.ToString(), // Chuyển size sang string
                    ImageUrl = productSize.Product.ImageUrl
                };
                _context.CartItems.Add(cartItem);
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            TempData["Message"] = "Sản phẩm đã được thêm vào giỏ hàng.";
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            // Lấy ID người dùng từ Claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Dangnhap", "Users");
            }

            // Tìm CartItem trong cơ sở dữ liệu dựa trên `id` và `userId`
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemId == id && ci.Cart.UserId == int.Parse(userId) && ci.Cart.IsActive);

            if (cartItem != null)
            {
                // Xóa CartItem khỏi cơ sở dữ liệu
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync(); // Lưu thay đổi vào cơ sở dữ liệu
                TempData["SuccessMessage"] = "Sản phẩm đã được xóa khỏi giỏ hàng.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm trong giỏ hàng.";
            }

            return RedirectToAction("Index");
        }


        //Cập nhât số lượng
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            if (quantity <= 0)
            {
                return Json(new { success = false, message = "Số lượng phải lớn hơn 0." });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện thao tác này." });
            }

            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart.UserId == int.Parse(userId) && ci.Cart.IsActive);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
            }

            // Kiểm tra số lượng tồn kho
            var productSize = await _context.ProductSizes.FirstOrDefaultAsync(ps => ps.ProductId == cartItem.ProductId && ps.Size == cartItem.Size);
            if (productSize == null || quantity > productSize.Quantity)
            {
                return Json(new { success = false, message = $"Không đủ hàng trong kho. Chỉ còn {productSize?.Quantity ?? 0} sản phẩm." });
            }

            // Cập nhật số lượng trong giỏ hàng
            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();

            // Tính lại tổng tiền
            var totalPrice = cartItem.Quantity * cartItem.PriceAtTime;
            var cartTotal = await _context.CartItems
                .Where(ci => ci.Cart.UserId == int.Parse(userId) && ci.Cart.IsActive)
                .SumAsync(ci => ci.Quantity * ci.PriceAtTime);

            return Json(new { success = true, totalPrice, cartTotal });
        }



        //Checkout
        [HttpGet]
        public IActionResult Checkout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Dangnhap", "Users");
            }

            var cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.UserId == int.Parse(userId) && c.IsActive);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "Carts");
            }

            // Tính tổng tiền trước giảm giá
            var subtotal = cart.CartItems.Sum(ci => ci.PriceAtTime * ci.Quantity);

            // Tính tổng giảm giá từ sản phẩm
            var totalDiscount = cart.CartItems.Sum(ci =>
                (ci.PriceAtTime * ci.Quantity * ci.Product.Discount) / 100);

            // Tổng tiền sau khi áp dụng giảm giá
            var shippingFee = 50000;
            var total = subtotal - totalDiscount + shippingFee;

            // Chuẩn bị ViewModel
            var model = new CheckoutViewModel
            {
                CartItems = cart.CartItems
                    .Select(ci => new CartItemViewModel
                    {
                        ProductName = ci.Product.ProductName,
                        Quantity = ci.Quantity,
                        PriceAtTime = ci.PriceAtTime,
                        Discount = ci.Product.Discount // Lấy giá trị giảm giá từ Product
                    }).ToList(),
                DiscountAmount = (int)totalDiscount, // Tổng giảm giá
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Lấy lại thông tin giỏ hàng để hiển thị khi có lỗi xác thực
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    var cart = _context.Carts
                        .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                        .FirstOrDefault(c => c.UserId == int.Parse(userId) && c.IsActive);

                    if (cart != null)
                    {
                        model.CartItems = cart.CartItems
                            .Select(ci => new CartItemViewModel
                            {
                                ProductName = ci.Product.ProductName,
                                Quantity = ci.Quantity,
                                PriceAtTime = ci.PriceAtTime,
                                Discount = ci.Product.Discount // Lấy giá trị giảm giá từ sản phẩm
                            }).ToList();
                    }
                }

                return View(model); // Trả lại View cùng lỗi xác thực và thông tin giỏ hàng
            }

            var userIdValidated = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdValidated))
            {
                return RedirectToAction("Dangnhap", "Users");
            }

            var cartValidated = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c => c.UserId == int.Parse(userIdValidated) && c.IsActive);

            if (cartValidated == null || !cartValidated.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "Carts");
            }

            // Tính tổng tiền trước giảm giá
            var subtotal = cartValidated.CartItems.Sum(ci => ci.PriceAtTime * ci.Quantity);

            // Tính tổng giảm giá từ sản phẩm
            var totalDiscount = cartValidated.CartItems.Sum(ci =>
                (ci.PriceAtTime * ci.Quantity * ci.Product.Discount) / 100);

            // Tổng tiền sau khi áp dụng giảm giá
            var shippingFee = 50000;
            var total = subtotal - totalDiscount + shippingFee;

            // Lưu thông tin đơn hàng
            var order = new Order
            {
                UserId = int.Parse(userIdValidated),
                OrderDate = DateTime.Now,
                TotalAmount = total,
                OrderStatus = "Pending",
                ShippingAddress = model.ShippingAddress,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            // Lưu các sản phẩm trong đơn hàng và trừ số lượng trong kho
            foreach (var item in cartValidated.CartItems)
            {
                // Tìm sản phẩm trong kho
                var productSize = _context.ProductSizes
                    .FirstOrDefault(ps => ps.ProductId == item.ProductId && ps.Size == item.Size);

                if (productSize != null)
                {
                    // Kiểm tra tồn kho
                    if (productSize.Quantity < item.Quantity)
                    {
                        TempData["ErrorMessage"] = $"Sản phẩm {item.Product.ProductName} không đủ số lượng trong kho.";
                        return RedirectToAction("Index", "Carts");
                    }

                    // Trừ số lượng tồn kho
                    productSize.Quantity -= item.Quantity;
                    _context.ProductSizes.Update(productSize);
                }

                // Thêm vào OrderItems
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.PriceAtTime,
                    Size = item.Size
                };
                _context.OrderItems.Add(orderItem);
            }

            // Xóa các sản phẩm khỏi giỏ hàng
            _context.CartItems.RemoveRange(cartValidated.CartItems);
            cartValidated.IsActive = false;
            _context.Carts.Update(cartValidated);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Đơn hàng của bạn đã được tạo thành công.";
            return RedirectToAction("Index", "Orders");
        }




        // GET: Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CartId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Carts/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartId,UserId,CreatedDate,IsActive")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", cart.UserId);
            return View(cart);
        }

        // GET: Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", cart.UserId);
            return View(cart);
        }

        // POST: Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartId,UserId,CreatedDate,IsActive")] Cart cart)
        {
            if (id != cart.CartId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.CartId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", cart.UserId);
            return View(cart);
        }

        // GET: Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CartId == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.CartId == id);
        }

    }
}
