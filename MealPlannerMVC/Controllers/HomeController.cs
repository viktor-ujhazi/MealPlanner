using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MealPlannerMVC.Models;
using MealPlannerMVC.Services;

namespace MealPlannerMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IRecipesService _recipesService;

        public HomeController(ILogger<HomeController> logger, IRecipesService recipesService)
        {
            _logger = logger;
            _recipesService = recipesService;
        }

        public IActionResult Index()
        {

            ViewBag.Recipe = _recipesService.GetRecipe(1);
           
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult RecipeDetails(int id)
        {

            ViewBag.Recipe = _recipesService.GetRecipe(id);
            return View();
        }
    }
}
