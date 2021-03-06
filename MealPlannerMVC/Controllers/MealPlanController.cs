﻿using System;
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
        private readonly IShoppingListsService _shoppingListsService;


        public MealPlanController(ILogger<MealPlanController> logger, IRecipesService recipesService, IShoppingListsService shoppingListsService)
        {
            _logger = logger;
            _recipesService = recipesService;
            _shoppingListsService = shoppingListsService;

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

        public IActionResult AddToPlannedMeals()
        {
            int userID = Convert.ToInt32(HttpContext.User.FindFirst("Id").Value);
            int recipeID = Convert.ToInt32(Request.Form["recipeID"]);
            var recipeNames = _shoppingListsService.AddToPlannedMeals(userID, recipeID);

            return Json("OK");
        }

        //public IActionResult AddToShoppingList()
        //{
        //    int userID = Convert.ToInt32(HttpContext.User.FindFirst("Id").Value);
        //    int recipeID = Convert.ToInt32(Request.Form["recipeID"]);
        //    var ingredients = _shoppingListsService.AddToShoppingList(userID, recipeID);

        //    return Json(ingredients);

        //}

        public IActionResult GetShoppingList()
        {
            int userID = Convert.ToInt32(HttpContext.User.FindFirst("Id").Value);
            var ingredients = _shoppingListsService.GetShoppingList(userID);

            return Json(ingredients);


        }
        
        public IActionResult GetPlannedRecipes()
        {
            int userID = Convert.ToInt32(HttpContext.User.FindFirst("Id").Value);
            var recipes = _shoppingListsService.GetPlannedRecipes(userID);

            return Json(recipes);


        }
        public IActionResult DeleteFromShoppingList() 
        {
            int ingredientID = Convert.ToInt32(Request.Form["ingredientID"]);
            _shoppingListsService.DeleteFromShoppingList(ingredientID);

            return Json("OK");
        }
        
        public IActionResult DeleteFromPlannedRecipes()
        {
            int userID = Convert.ToInt32(HttpContext.User.FindFirst("Id").Value);
            int recipeID = Convert.ToInt32(Request.Form["recipeID"]);
            _shoppingListsService.DeleteFromPlannedRecipes(userID, recipeID);

            return Json("OK");
        }

        public IActionResult GetPrices()
        {
            int userID = Convert.ToInt32(HttpContext.User.FindFirst("Id").Value);
            var prices = _shoppingListsService.GetPriceForIngredients(userID); //get_price_for_ingredient(u_id INTEGER)

            return Json(prices);


        }
        


    }
}