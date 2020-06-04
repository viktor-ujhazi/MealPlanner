using MealPlannerMVC.Models;
using System.Collections.Generic;

namespace MealPlannerMVC.Services
{
    public interface IRecipesService
    {
        RecipeModel GetRecipe(int id);
        List<RecipeModel> GetRecipes();
        List<RecipeIngredientsModel> GetALLIngredients();
        List<string> GetALLMeasurementUnits();
        void AddRecipe(string jsonRecipe);
        List<RecipeModel> SearchRecipes(string recipename);
        List<RecipeIngredientsModel> GetIngredients(int id);
    }
}