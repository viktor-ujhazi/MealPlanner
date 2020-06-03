using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MealPlannerMVC.Models
{
    public class ShopInventoryModel
    {
        public int ShopID { get; set; }
        public int ItemID { get; set; }
        public int IngredientID { get; set; }
        public string ItemName { get; set; }
        public string ItemCategory { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
    }
}
