using MealPlannerMVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MealPlannerMVC.Services
{
    public class SQLUserInventoryService : IUserInventoryService
    {
        private readonly IDbConnection _connection;
        public SQLUserInventoryService(IDbConnection connection)
        {
            _connection = connection;
        }

        public List<UserInventoryModel> GetAllItem(int userID)
        {
            List<UserInventoryModel> inventoryItems = new List<UserInventoryModel>();
            using var command = _connection.CreateCommand();
            command.CommandText = $"SELECT * FROM user_inventory JOIN ingredients ON ingredients.ingredient_id = user_inventory.ingredient_id WHERE user_id = {userID}";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                UserInventoryModel inventoryItem = new UserInventoryModel
                {
                    UserID = (int)reader["user_id"],
                    IngredientName = (string)reader["ingredient_name"],
                    IngredientID = (int)reader["ingredient_id"]

                };
                inventoryItems.Add(inventoryItem);
            }
            return inventoryItems;
        }
    }
}
