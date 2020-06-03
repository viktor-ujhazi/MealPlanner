using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MealPlannerMVC.Models;
using MealPlannerMVC.Services;
using Microsoft.AspNetCore.Http;
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
        public IActionResult Recipes(List<RecipeModel> recipes)
        {
            if (recipes.Count == 0)
            {
                recipes = _recipesService.GetRecipes();
            }
            return View(recipes);
        }

        public IActionResult Ingredients(List<RecipeIngredientsModel> recipes)
        {
            if (recipes.Count == 0)
            {
                recipes = _recipesService.GetALLIngredients();
            }
            return View(recipes);
        }

        public IActionResult MeasurementUnits(List<string> measurementUnits)
        {
            if (measurementUnits.Count == 0)
            {
                measurementUnits = _recipesService.GetALLMeasurementUnits();
            }
            ViewBag.MeasurementUnits = measurementUnits;
            return View();
        }
        public IActionResult NewRecipe()
        {
            
            return View();
        }
        
        public IActionResult AddRecipe()
        {
            
            var recipeName = Request.Form["recipeName"];
            var recipeDescription = Request.Form["recipeDescription"];
            var steps = Request.Form["recipeSteps"];
            var ingredients = Request.Form["recipeIngredients"];
                       
            string jsonString = $"{{\"RecipeID\":0,\"RecipeName\":\"{recipeName}\", \"Description\":\"{recipeDescription}\", " +
                $"\"Steps\": {steps} ," +
                $"\"Ingredients\": {ingredients}}}";
                
            
            _recipesService.AddRecipe(jsonString);
                     

            return Json("OK");
        }

    }
}