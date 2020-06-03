namespace MealPlannerMVC.Models
{
    public class RecipeIngredientsModel
    {
        public int IngredientID { get; set; }
        public string IngredientName { get; set; }
        public double MeasurementQuantity { get; set; }
        public string MeasurementUnit { get; set; }
        
    }
}