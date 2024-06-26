﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ProductDemo.Data;
using ProductDemo.Models;
using ProductDemo.Services;
using ProductDemo.ViewModels;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ProductDemo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IInvoiceService _invoiceService;
        private readonly ApplicationDbContext _context;

        public HomeController(IProductService productService, IInvoiceService invoiceService, ApplicationDbContext context)
        {
            _productService = productService;
            _invoiceService = invoiceService;
            _context = context;
        }

        public async Task<IActionResult> Index(string search = "", int page = 1)
        {
            var viewModel = await _productService.GetProductsAsync(search, page);
            return View(viewModel);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }
            var products = await _context.Product
                                        .Include(p => p.ProductCategory)
                                        .FirstOrDefaultAsync(x => x.ProductId == id);
            if (products == null)
            {
                return NotFound();
            }
            return View(products);
        }






        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

