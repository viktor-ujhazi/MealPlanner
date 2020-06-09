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

        private int[] GetIngredientIDsByRecipeID(int id)
        {
            List<int> ingredientIDs = new List<int>();
            using var command = _connection.CreateCommand();
            command.CommandText = $"SELECT ingredient_id " +
                                    "FROM recipe_ingredients " +
                                    $"WHERE recipe_ingredients.recipe_id = {id};";

            var param = command.CreateParameter();
            param.ParameterName = "recipe_id";
            param.Value = id;

            command.Parameters.Add(param);
            using var reader = command.ExecuteReader();


            while (reader.Read())
            {
                ingredientIDs.Add((int)reader["ingredient_id"]);
            }

            int[] ingredientIDsArray = ingredientIDs.ToArray();

            return ingredientIDsArray;

        }
        public List<RecipeIngredientsModel> AddToShoppingList(int userID, int recipeID)
        {
            int[] ingredientIDs = GetIngredientIDsByRecipeID(recipeID);
            using var command = _connection.CreateCommand();

            var UserIdParam = command.CreateParameter();
            UserIdParam.ParameterName = "u_id";
            UserIdParam.Value = userID;

            var IngredientsIdParam = command.CreateParameter();
            IngredientsIdParam.ParameterName = "i_id";
            IngredientsIdParam.Value = ingredientIDs;


            command.CommandText = "SELECT add_to_shopping_list(@u_id, @i_id)";
            command.Parameters.Add(UserIdParam);
            command.Parameters.Add(IngredientsIdParam);

            command.ExecuteNonQuery();

            List<RecipeIngredientsModel> shoppingList = GetShoppingList(userID);
            return shoppingList;

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
    }
}
