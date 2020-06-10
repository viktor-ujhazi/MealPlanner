using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MealPlannerMVC.Models
{
    public class IngredientPriceModel
    {
        public int IngredientID { get; set; }
        public string IngredientName { get; set; }

        public int ShopID { get; set; }
        public string ShopName { get; set; }
        public int ItemID { get; set; }
        
        public string ItemName { get; set; }
        
        public double Price { get; set; }
        public string Currency { get; set; }
    }
}
