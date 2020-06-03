using MealPlannerMVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MealPlannerMVC.Services
{
    public class SQLShopInventoryService : IShopInventoryService
    {
        private readonly IDbConnection _connection;

        public SQLShopInventoryService(IDbConnection connection)
        {
            _connection = connection;
        }

        public List<ShopInventoryModel> GetAllItem(int shopID)
        {
            List<ShopInventoryModel> inventoryItems = new List<ShopInventoryModel>();
            using var command = _connection.CreateCommand();
            command.CommandText = $"SELECT * FROM shop_inventory JOIN ingredients ON ingredients.ingredient_id = shop_inventory.ingredient_id WHERE shop_id = {shopID}";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                ShopInventoryModel inventoryItem = new ShopInventoryModel
                {
                    ItemID = (int)reader["item_id"],
                    ShopID = (int)reader["shop_id"],
                    ItemName = (string)reader["item_name"],
                    Price = (double)reader["price"],
                    Currency = (string)reader["currency"],
                    ItemCategory = (string)reader["ingredient_name"],
                    IngredientID = (int)reader["ingredient_id"]
                    
                };
                inventoryItems.Add(inventoryItem);
            }
            return inventoryItems;
        }

        public ShopInventoryModel GetItemByID(int inventoryItemID)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = $"SELECT * FROM shop_inventory JOIN ingredients ON ingredients.ingredient_id = shop_inventory.ingredient_id WHERE item_id = {inventoryItemID}";

            using var reader = command.ExecuteReader();

            reader.Read();
            ShopInventoryModel inventoryItem = new ShopInventoryModel
            {
                ItemID = (int)reader["item_id"],
                ShopID = (int)reader["shop_id"],
                ItemName = (string)reader["item_name"],
                Price = (double)reader["price"],
                Currency = (string)reader["currency"],
                ItemCategory = (string)reader["ingredient_name"],
                IngredientID = (int)reader["ingredient_id"]
                
            };
            return inventoryItem;
        }

        public void UpdateItem(ShopInventoryModel inventoryItem)
        {

        }
    }
}
