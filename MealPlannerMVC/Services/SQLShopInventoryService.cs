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

        public void UpdateItem(int inventoryItemID, ShopInventoryModel inventoryItem)
        {
            int ingredientID = GetIngredientID(inventoryItem.ItemCategory);


            using var command = _connection.CreateCommand();

            var ingredientIDParam = command.CreateParameter();
            ingredientIDParam.ParameterName = "ingredient_id";
            ingredientIDParam.Value = ingredientID;

            var itemNameParam = command.CreateParameter();
            itemNameParam.ParameterName = "item_name";
            itemNameParam.Value = inventoryItem.ItemName;

            var itemIDParam = command.CreateParameter();
            itemIDParam.ParameterName = "item_id";
            itemIDParam.Value = inventoryItemID;

            var priceParam = command.CreateParameter();
            priceParam.ParameterName = "price";
            priceParam.Value = inventoryItem.Price;

            var currencyParam = command.CreateParameter();
            currencyParam.ParameterName = "currency";
            currencyParam.Value = inventoryItem.Currency;




            command.CommandText = @"UPDATE shop_inventory SET ingredient_id = @ingredient_id , item_name = @item_name, price = @price, currency = @currency WHERE item_id = @item_id";
            command.Parameters.Add(ingredientIDParam);
            command.Parameters.Add(itemNameParam);
            
            command.Parameters.Add(priceParam);
            command.Parameters.Add(currencyParam);
            command.Parameters.Add(itemIDParam);


            command.ExecuteNonQuery();
        }
        public void AddItem(ShopInventoryModel inventoryItem) 
        {
            int ingredientID = GetIngredientID(inventoryItem.ItemCategory);


            using var command = _connection.CreateCommand();

            var ingredientIDParam = command.CreateParameter();
            ingredientIDParam.ParameterName = "ingredient_id";
            ingredientIDParam.Value = ingredientID;

            var itemNameParam = command.CreateParameter();
            itemNameParam.ParameterName = "item_name";
            itemNameParam.Value = inventoryItem.ItemName;

            var shopIDParam = command.CreateParameter();
            shopIDParam.ParameterName = "shop_id";
            shopIDParam.Value = inventoryItem.ShopID;

            var priceParam = command.CreateParameter();
            priceParam.ParameterName = "price";
            priceParam.Value = inventoryItem.Price;

            var currencyParam = command.CreateParameter();
            currencyParam.ParameterName = "currency";
            currencyParam.Value = inventoryItem.Currency;




            command.CommandText = @"INSERT INTO shop_inventory (ingredient_id, item_name, shop_id, price, currency) VALUES (@ingredient_id, @item_name, @shop_id, @price, @currency)";
            command.Parameters.Add(ingredientIDParam);
            command.Parameters.Add(itemNameParam);
            command.Parameters.Add(shopIDParam);
            command.Parameters.Add(priceParam);
            command.Parameters.Add(currencyParam);
           

            command.ExecuteNonQuery();
        }
        private int GetIngredientID(string ingredientName)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = $"SELECT check_ingredient('{ingredientName}')";
            using var reader = command.ExecuteReader();

            reader.Read();
            int ingredientID = (int)reader["check_ingredient"];
            return ingredientID;
        }

        public void DeleteItem(int id)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = $"DELETE FROM shop_inventory WHERE item_id = {id}";
            command.ExecuteNonQuery();
        }

    }
}
