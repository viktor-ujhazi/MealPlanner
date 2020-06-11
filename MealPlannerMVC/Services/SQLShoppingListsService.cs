using MealPlannerMVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MealPlannerMVC.Services
{
    public class SQLShoppingListsService : IShoppingListsService
    {
        private readonly IDbConnection _connection;

        public SQLShoppingListsService(IDbConnection connection)
        {
            _connection = connection;
        }

        //private int[] GetIngredientIDsByRecipeID(int id)
        //{
        //    List<int> ingredientIDs = new List<int>();
        //    using var command = _connection.CreateCommand();
        //    command.CommandText = $"SELECT ingredient_id " +
        //                            "FROM recipe_ingredients " +
        //                            $"WHERE recipe_ingredients.recipe_id = {id};";

        //    var param = command.CreateParameter();
        //    param.ParameterName = "recipe_id";
        //    param.Value = id;

        //    command.Parameters.Add(param);
        //    using var reader = command.ExecuteReader();


        //    while (reader.Read())
        //    {
        //        ingredientIDs.Add((int)reader["ingredient_id"]);
        //    }

        //    int[] ingredientIDsArray = ingredientIDs.ToArray();

        //    return ingredientIDsArray;

        //}

        public List<RecipeModel> AddToPlannedMeals(int userID, int recipeID)
        {
            using var command = _connection.CreateCommand();

            var UserIdParam = command.CreateParameter();
            UserIdParam.ParameterName = "user_id";
            UserIdParam.Value = userID;

            var RecipeIdParam = command.CreateParameter();
            RecipeIdParam.ParameterName = "recipe_id";
            RecipeIdParam.Value = recipeID;

            
            command.CommandText = "INSERT INTO planned_meals (user_id, recipe_id) VALUES (@user_id, @recipe_id)";
            command.Parameters.Add(UserIdParam);
            command.Parameters.Add(RecipeIdParam);

            command.ExecuteNonQuery();

            List<RecipeModel> plannedRecipes = GetPlannedRecipes(userID);
            return plannedRecipes;




        }
        //public List<RecipeIngredientsModel> AddToShoppingList(int userID, int recipeID)
        //{
        //    int[] ingredientIDs = GetIngredientIDsByRecipeID(recipeID);
        //    using var command = _connection.CreateCommand();

        //    var UserIdParam = command.CreateParameter();
        //    UserIdParam.ParameterName = "u_id";
        //    UserIdParam.Value = userID;

        //    var IngredientsIdParam = command.CreateParameter();
        //    IngredientsIdParam.ParameterName = "i_id";
        //    IngredientsIdParam.Value = ingredientIDs;


        //    command.CommandText = "SELECT add_to_shopping_list(@u_id, @i_id)";
        //    command.Parameters.Add(UserIdParam);
        //    command.Parameters.Add(IngredientsIdParam);

        //    command.ExecuteNonQuery();

        //    List<RecipeIngredientsModel> shoppingList = GetShoppingList(userID);
        //    return shoppingList;

        //}

        public List<RecipeModel> GetPlannedRecipes(int userID)
        {
            List<RecipeModel> plannedRecipes = new List<RecipeModel>();
            using var command = _connection.CreateCommand();
            command.CommandText = $"SELECT * FROM recipes WHERE recipe_id in" +
                $"(SELECT recipe_id FROM planned_meals WHERE user_id = {userID})";





            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                RecipeModel recipe = new RecipeModel
                {
                    RecipeID = (int)reader["recipe_id"],
                    RecipeName = (string)reader["recipe_name"]
                                        

                };

                plannedRecipes.Add(recipe);
            }
            return plannedRecipes;
        }

        public List<RecipeIngredientsModel> GetShoppingList(int userID)
        {
            List<RecipeIngredientsModel> Ingredients = new List<RecipeIngredientsModel>();
            using var command = _connection.CreateCommand();
            command.CommandText = $"SELECT * FROM ingredients WHERE ingredient_id in" +
                $"(SELECT ingredient_id FROM shopping_list WHERE user_id = {userID})";





            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                RecipeIngredientsModel ingredient = new RecipeIngredientsModel
                {
                    IngredientID = (int)reader["ingredient_id"],
                    IngredientName = (string)reader["ingredient_name"]

                };

                Ingredients.Add(ingredient);
            }
            return Ingredients;
        }

        public void DeleteFromShoppingList(int ingredientID)
        {
            using var command = _connection.CreateCommand();

            var IngredientIdParam = command.CreateParameter();
            IngredientIdParam.ParameterName = "ingredient_id";
            IngredientIdParam.Value = ingredientID;

            command.CommandText = $"DELETE FROM shopping_list WHERE ingredient_id={ingredientID}";

            command.Parameters.Add(IngredientIdParam);

            command.ExecuteNonQuery();
        }
        public void DeleteFromPlannedRecipes(int userID, int recipeID)
        {
            using var command = _connection.CreateCommand();

            var recipeIdParam = command.CreateParameter();
            recipeIdParam.ParameterName = "ingredient_id";
            recipeIdParam.Value = recipeID;

            command.CommandText = $"DELETE FROM planned_meals WHERE recipe_id={recipeID} AND user_id={userID}";

            command.Parameters.Add(recipeIdParam);

            command.ExecuteNonQuery();
        }

        public List<IngredientPriceModel> GetPriceForIngredients(int userID)
        {
            List<IngredientPriceModel> prices = new List<IngredientPriceModel>();
            using var command = _connection.CreateCommand();
            command.CommandText = $"select distinct on (shop_inventory.ingredient_id) shop_inventory.ingredient_id, " +
                $"shop_inventory.item_id, min(price) as price, shop_inventory.item_name, shop_inventory.shop_id, shop_inventory.currency " +
                $"from shop_inventory " +
                $"join shopping_list on shopping_list.ingredient_id = shop_inventory.ingredient_id " +
                $"where shopping_list.user_id = {userID} " +
                $"Group by shop_inventory.ingredient_id, shop_inventory.item_id, shop_inventory.item_name, shop_inventory.shop_id, shop_inventory.currency; ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                IngredientPriceModel price = new IngredientPriceModel
                {
                    IngredientID = (int)reader["ingredient_id"],
                    //IngredientName = (string)reader["recipe_name"],
                    ItemID = (int)reader["item_id"],
                    ItemName = (string)reader["item_name"],
                    ShopID = (int)reader["shop_id"],
                    //ShopName = (string)reader["recipe_name"],
                    Price = (double)reader["price"],
                    Currency = (string)reader["currency"],
                                       
                };

                prices.Add(price);
            }
            command.Dispose();
            reader.Close();
            GetIngredientsNameByID(prices);
            GetShopsNameByID(prices);
            return prices;
        }

        private void GetIngredientsNameByID(List<IngredientPriceModel> prices)
        {
            foreach (var item in prices)
            {
                using var command = _connection.CreateCommand();
                command.CommandText = $"SELECT ingredient_name FROM ingredients WHERE ingredient_id={item.IngredientID}";
                using var reader = command.ExecuteReader();
                reader.Read();
                item.IngredientName = (string)reader["ingredient_name"];
                reader.Close();
                command.Dispose();
            }
        }
        private void GetShopsNameByID(List<IngredientPriceModel> prices)
        {
            foreach (var item in prices)
            {
                using var command = _connection.CreateCommand();
                command.CommandText = $"SELECT username FROM users WHERE user_id={item.ShopID}";
                using var reader = command.ExecuteReader();
                reader.Read();
                item.ShopName = (string)reader["username"];
                reader.Close();
                command.Dispose();
            }
        }
    }
}
