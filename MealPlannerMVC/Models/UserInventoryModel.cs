using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MealPlannerMVC.Models
{
    public class UserInventoryModel
    {
        public int UserID { get; set; }
       
        public int IngredientID { get; set; }
        public string IngredientName { get; set; }
        
        
    }
}
