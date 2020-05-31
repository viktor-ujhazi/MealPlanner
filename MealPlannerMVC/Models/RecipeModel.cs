using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MealPlannerMVC.Models
{
    public class RecipeModel
    {
        public int RecipeID { get; set; }
        public string RecipeName { get; set; }
        public string Description { get; set; }
        public List<RecipeSteps> Steps { get; set; }
        public List<RecipeIngredientsModel> Ingredients { get; set; }
        
    }
}
