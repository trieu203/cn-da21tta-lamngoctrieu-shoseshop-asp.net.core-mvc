using chuyenNganh.websiteBanGiay.Helpers;
using chuyenNganh.websiteBanGiay.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace chuyenNganh.websiteBanGiay.Models.Components
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItemViewModel>>(Constand.Cart_Key) 
                ?? new List<CartItemViewModel>();
            return View("CartPanel", new Cartmodel
            {
                Quantity = cart.Sum(p => p.Quantity),
                Total = (double)cart.Sum(p => p.PriceAtTime)
            });
        }

    }
}
