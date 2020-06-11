using MealPlannerMVC.Models;
using System.Collections.Generic;

namespace MealPlannerMVC.Services
{
    public interface IShopInventoryService
    {
        List<ShopInventoryModel> GetAllItem(int shopID);
        ShopInventoryModel GetItemByID(int inventoryItemID);
        void UpdateItem(int inventoryItemID, ShopInventoryModel inventoryItem);
        void AddItem(ShopInventoryModel inventoryItem);
        void DeleteItem(int id);
    }
}