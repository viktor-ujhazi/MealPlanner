using MealPlannerMVC.Models;
using System.Collections.Generic;

namespace MealPlannerMVC.Services
{
    public interface IShoppingListsService
    {
        //List<RecipeIngredientsModel> AddToShoppingList(int userID, int recipeID);
        void DeleteFromShoppingList(int ingredientID);
        List<RecipeIngredientsModel> GetShoppingList(int userID);
        List<RecipeModel> AddToPlannedMeals(int userID, int recipeID);
        List<RecipeModel> GetPlannedRecipes(int userID);
        void DeleteFromPlannedRecipes(int recipeID);
        List<IngredientPriceModel> GetPriceForIngredients(int userID);
    }
}