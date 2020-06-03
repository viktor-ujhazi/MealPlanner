using MealPlannerMVC.Models;
using System.Collections.Generic;

namespace MealPlannerMVC.Services
{
    public interface IRecipesService
    {
        RecipeModel GetRecipe(int id);
        public List<RecipeModel> GetRecipes();
        public List<RecipeIngredientsModel> GetALLIngredients();
        public List<string> GetALLMeasurementUnits();
        public void AddRecipe(string jsonRecipe);
    }
}