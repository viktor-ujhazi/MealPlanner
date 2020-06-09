using MealPlannerMVC.Models;
using System.Collections.Generic;

namespace MealPlannerMVC.Services
{
    public interface IShoppingListsService
    {
        List<RecipeIngredientsModel> AddToShoppingList(int userID, int recipeID);
        void DeleteFromShoppingList(int ingredientID);
        List<RecipeIngredientsModel> GetShoppingList(int userID);
    }
}