using chuyenNganh.websiteBanGiay.Helpers;
using chuyenNganh.websiteBanGiay.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace chuyenNganh.websiteBanGiay.Models.Components
{
    public class WishListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var wisshList = HttpContext.Session.Get<List<WishListViewModel>>(Constand.WishList_Key)
                ?? new List<WishListViewModel>();
            return View("WishListPanel", new WishListModel
            {
                Quantity = wisshList.Sum(p => p.Quantity),
                Total = (double)wisshList.Sum(p => p.WishListId)
            });
        }

    }
}
