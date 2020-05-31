using MealPlannerMVC.Models;
using System.Collections.Generic;

namespace MealPlannerMVC.Services
{
    public interface IRecipesService
    {
        RecipeModel GetRecipe(int id);
        public List<RecipeModel> GetRecipes();
    }
}