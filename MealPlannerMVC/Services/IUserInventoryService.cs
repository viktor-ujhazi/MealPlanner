using MealPlannerMVC.Models;
using System.Collections.Generic;

namespace MealPlannerMVC.Services
{
    public interface IUserInventoryService
    {

        public List<UserInventoryModel> GetAllItem(int userID);
    }
}