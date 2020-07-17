using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edura.WebUI.Entity;
using Edura.WebUI.Infrastructure;
using Edura.WebUI.Models;
using Edura.WebUI.Repository.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edura.WebUI.Controllers
{
    public class CartController : Controller
    {
        public IUnitOfWork unitofWork;

        public CartController(IUnitOfWork _unitofWork)
        {
            unitofWork = _unitofWork;
        }

        public IActionResult Index()
        {
            return View(GetCart());
        }
        public IActionResult AddToCart(int ProductId, int quantity=1)
        {
            var product = unitofWork.Products.Get(ProductId);

            if (product != null)
            {
                var cart = GetCart();

                cart.AddProduct(product, quantity);
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }
        public IActionResult RemoveFromCart(int ProductId)
        {
            var product = unitofWork.Products.Get(ProductId);

            if (product != null)
            {
                var cart = GetCart();
                cart.RemoveProduct(product);
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public IActionResult Checkout(OrderDetails model)
        {
            var cart = GetCart();
            if (cart.Products.Count == 0)
            {
                ModelState.AddModelError("UrunYokModel", "There is no product in your cart.");
            }
            if (ModelState.IsValid)
            {
                SaveOrder(cart,model);
                cart.ClearAll();
                SaveCart(cart);
                return View("Completed");
            }
            return View(model);
        }

        private void SaveOrder(CartModel cart, OrderDetails orderDetails)
        {
            var order = new Order();
            order.OrderNumber = "A" + (new Random()).Next(11111, 99999).ToString();
            order.Total = cart.TotalPrice();
            order.OrderDate = DateTime.Now;
            order.OrderState = EnumOrderState.Waiting;
            order.UserName = User.Identity.Name;

            order.AddressTitle = orderDetails.AddressTitle;
            order.Address = orderDetails.Address;
            order.City = orderDetails.City;
            order.District = orderDetails.District;
            order.Telephone = orderDetails.Telephone;

            foreach (var product in cart.Products)
            {
                var orderline = new OrderLine();
                orderline.Quantity = product.Quantity;
                orderline.Price = product.Product.Price;
                orderline.ProductId = product.Product.ProductId;

                order.OrderLines.Add(orderline);

            }
            unitofWork.Orders.Add(order);
            unitofWork.SaveChanges();
        }

        private void SaveCart(CartModel cart)
        {
            HttpContext.Session.SetJson("Cart", cart);
        }

        private CartModel GetCart()
        {
            return HttpContext.Session.GetJson<CartModel>("Cart") ?? new CartModel();
        }
    }
}