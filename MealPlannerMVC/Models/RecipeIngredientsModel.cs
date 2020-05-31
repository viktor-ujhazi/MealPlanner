namespace MealPlannerMVC.Models
{
    public class RecipeIngredientsModel
    {
        public int IngredientID { get; set; }
        public string IngredientName { get; set; }
        public IngredientQuantityModel Quantity { get; set; }
    }
}