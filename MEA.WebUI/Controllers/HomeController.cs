using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Edura.WebUI.Models;
using Edura.WebUI.Repository.Abstract;
using Edura.WebUI.Entity;

namespace Edura.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private IProductRepository repository;
        private IUnitOfWork unitOfWork;

        public HomeController(IProductRepository _repository, IUnitOfWork _unitOfWork)
        {
            repository = _repository;
            unitOfWork = _unitOfWork;
        }

        public IActionResult Index()
        {
            return View(unitOfWork.Products.GetAll().Where(i => i.IsApproved && i.IsHome).ToList());
        }
        public IActionResult Details(int id)
        {
            return View(repository.Get(id));
        }
        public IActionResult Create()
        {
            var product = new Product() { ProductName="Yeni Ürün", Price=1000 };
            unitOfWork.Products.Add(product);
            unitOfWork.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
