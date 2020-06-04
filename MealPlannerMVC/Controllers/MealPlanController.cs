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
    public class MealPlanController : Controller
    {

        private readonly ILogger<MealPlanController> _logger;
        private readonly IRecipesService _recipesService;

        public MealPlanController(ILogger<MealPlanController> logger, IRecipesService recipesService)
        {
            _logger = logger;
            _recipesService = recipesService;
        }
        public IActionResult MealPlanning()
        {
            return View();
        }

        public IActionResult SearchRecipeName()
        {
            var recipeName = Request.Form["recipeName"];

            List<RecipeModel> recipeList = _recipesService.SearchRecipes(recipeName);


            return Json(recipeList);
        }
        public IActionResult GetIngredientsByRecipeID()
        {
            var recipeID = Convert.ToInt32(Request.Form["recipeID"]);
            var ingredients = _recipesService.GetIngredients(recipeID);
            return Json(ingredients);
        }
    }
}