using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MealPlannerMVC.Models;
using MealPlannerMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MealPlannerMVC.Controllers
{
    public class RecipeController : Controller
    {
        private readonly ILogger<RecipeController> _logger;
        private readonly IRecipesService _recipesService;

        public RecipeController(ILogger<RecipeController> logger, IRecipesService recipesService)
        {
            _logger = logger;
            _recipesService = recipesService;
        }
        public IActionResult RecipeDetails(int id)
        {
            
            ViewBag.Recipe = _recipesService.GetRecipe(id);
            return View();
        }
        public IActionResult List(List<RecipeModel> recipes)
        {
            if (recipes.Count == 0)
            {
                recipes = _recipesService.GetRecipes();
            }
            return View(recipes);
        }
        public IActionResult NewRecipe()
        {
            
            return View();
        }
        //[HttpPost]
        //public IActionResult NewRecipe(RecipeModel newRecipe)
        //{

        //    return RedirectToAction("RecipeDetails", new { recipeID });
        //}
        public IActionResult AddRecipe()
        {
            var title = Request.Form["title"];
            var content = Request.Form["content"];
            var userId = Convert.ToInt32(Request.Form["userID"]);

            //_sqlTaskService.AddTask(title, content, userId);
            //_sqlLogger.Log(userId, $"Task added, title: {title}, content: {content}");
            //return Json(_sqlTaskService.GetAllTask(userId));
            return Json("OK");
        }

    }
}