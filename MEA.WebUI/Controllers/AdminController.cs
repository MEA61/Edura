using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Edura.WebUI.Entity;
using Edura.WebUI.Models;
using Edura.WebUI.Repository.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Edura.WebUI.Controllers
{
    public class AdminController : Controller
    {
        private IUnitOfWork unitOfWork;

        public AdminController(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult EditCategory(int id)
        {
            var entity = unitOfWork.Categories.GetAll()
                                .Include(i => i.ProductCategories)
                                .ThenInclude(i => i.Product)
                                .Where(i => i.CategoryId == id)
                                .Select(i => new AdminEditCategoryModel()
                                {
                                    CategoryId = i.CategoryId,
                                    CategoryName = i.CategoryName,
                                    Products = i.ProductCategories.Select(a => new AdminEditCategoryProduct()
                                    {
                                        ProductId = a.ProductId,
                                        ProductName = a.Product.ProductName,
                                        Image = a.Product.Image,
                                        DateAdded = a.Product.DateAdded,
                                        IsApproved = a.Product.IsApproved,
                                        IsHome = a.Product.IsHome,
                                        IsFeatured = a.Product.IsFeatured
                                    }).ToList()
                                }).FirstOrDefault();
            return View(entity);
        }
        [HttpPost]
        public IActionResult EditCategory(Category entity)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Categories.Edit(entity);
                unitOfWork.SaveChanges();

                return RedirectToAction("CatalogList");
            }
            return View("Error");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCategory(int ProductId, int CategoryId)
        {
            if (ModelState.IsValid)
            {
                //silme
                unitOfWork.Categories.RemoveFromCategory(ProductId, CategoryId);
                unitOfWork.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        public IActionResult CatalogList()
        {
            var model = new CatalogListModel()
            {
                Categories = unitOfWork.Categories.GetAll().ToList(),
                Products = unitOfWork.Products.GetAll().ToList()
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCategory(Category entity)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Categories.Add(entity);
                unitOfWork.SaveChanges();

                return Ok(entity);
            }
            return BadRequest();
        }
        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(Product entity, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products", file.FileName);
                    var path_tn = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\products\\tn", file.FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);

                        entity.Image = file.FileName;
                    }
                    using (var stream = new FileStream(path_tn, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                entity.DateAdded = DateTime.Now;
                unitOfWork.Products.Add(entity);
                unitOfWork.SaveChanges();
                return RedirectToAction("CatalogList");
            }
            return View(entity);
        }
    }
}