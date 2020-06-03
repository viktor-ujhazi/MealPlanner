using MealPlannerMVC.Models;
using System.Collections.Generic;

namespace MealPlannerMVC.Services
{
    public interface IShopInventoryService
    {
        List<ShopInventoryModel> GetAllItem(int shopID);
        public ShopInventoryModel GetItemByID(int inventoryItemID);
        public void UpdateItem(ShopInventoryModel inventoryItem);
    }
}