using chuyenNganh.websiteBanGiay.Data;
using chuyenNganh.websiteBanGiay.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace chuyenNganh.websiteBanGiay.Models.Components
{
    public class MenuCategory : ViewComponent
    {
        private readonly ChuyenNganhContext db;
        public MenuCategory(ChuyenNganhContext context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Categories
                .Include(ca => ca.Products)
                .Select(ca => new MenuCategoryVM
                {
                    CategoryId = ca.CategoryId,
                    CategoryName = ca.CategoryName,
                    SoLuong = ca.Products.SelectMany(p => p.ProductSizes).Sum(ps => ps.Quantity)
                }).ToList();

            return View("_MenuCategory", data);
        }
    }
}
