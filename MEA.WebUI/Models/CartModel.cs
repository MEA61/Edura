using Edura.WebUI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edura.WebUI.Models
{
    public class CartModel
    {
        private List<CartLine> products = new List<CartLine>();
        public List<CartLine> Products => products;

        public void AddProduct(Product product, int quantity)
        {
            var prdct = products
                .Where(i => i.Product.ProductId == product.ProductId)
                .FirstOrDefault();
            if (prdct == null)
            {
                products.Add(new CartLine()
                {
                    Product = product,
                    Quantity = quantity
                });
            }
            else
            {
                prdct.Quantity += quantity;
            }

        }

        public void RemoveProduct(Product product)
        {
            products.RemoveAll(i => i.Product.ProductId == product.ProductId);
        }
        public double TotalPrice()
        {
            return products.Sum(i => i.Product.Price * i.Quantity);
        }
        public void ClearAll()
        {
            products.Clear();
        }
    }

    public class CartLine
    {
        public int CartLineId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
