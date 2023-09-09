using Microsoft.AspNetCore.Mvc;
using MvcPractical.Models;
using System.Diagnostics;
using Packt.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
namespace MvcPractical.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NorthwindContext db;

        public HomeController(ILogger<HomeController> logger, NorthwindContext injectedContext)
        {
            _logger = logger;
            db= injectedContext;
        }

        public IActionResult Index()
        {
            HomeIndexViewModel model = new
                (
                VisitorCount: Random.Shared.Next(1, 1001),
                Categories: db.Categories.ToList(),
                Products: db.Products.ToList()
                );
            return View(model);
        }
        [Authorize(Roles="Administrators")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ProductDetail(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("you must pass a product id in aroute,for example,Home/ProductDetail/21");
            }
            Product? model=db.Products.SingleOrDefault(p => p.ProductId == id);
            if(model is null)
            {
                return NotFound($"product id {id} not found");
            }
            return View(model);
        }
        public IActionResult ProductsThatCostMoreThan(decimal? price)
        {
            if(!price.HasValue) 
            {
               return BadRequest( "You must pass a product price in the query string,for example, / Home / ProductsThatCostMoreThan ? price = 50");
            }

            IEnumerable<Product> model = db.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.UnitPrice > price);
            if(!model.Any())
            {
                return NotFound($"no products cost more than {price:C} ");
            }
            ViewData["MaxPrice"] = price.Value.ToString("C");
            return View(model);
        }
    }
}