using MealPlannerMVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MealPlannerMVC.Services
{
    public class SQLRecipesService : IRecipesService
    {
        private readonly IDbConnection _connection;

        private RecipeModel RecipeModelFromData(IDataReader reader, List<RecipeSteps> steps, List<RecipeIngredientsModel> ingredients)
        {
            return new RecipeModel
            {
                RecipeID = (int)reader["recipe_id"],
                RecipeName = (string)reader["recipe_name"],
                Description = (string)reader["description"],
                Steps = steps,
                Ingredients = ingredients
            };
        }
        private RecipeSteps ToSteps(IDataReader reader)
        {
            return new RecipeSteps
            {
                StepNumber = (int)reader["step_number"],
                StepText = (string)reader["step_text"]
            };
        }
        private RecipeIngredientsModel ToIngredient(IDataReader reader)
        {
            IngredientQuantityModel quantity = new IngredientQuantityModel
            {
                MeasurementQuantity = (double)reader["quantity_amount"],
                MeasurementUnit = (string)reader["measurement_unit_text"]
            };
            return new RecipeIngredientsModel
            {
                IngredientName = (string)reader["ingredient_name"],
                Quantity = quantity

            };
        }


        public SQLRecipesService(IDbConnection connection)
        {
            _connection = connection;
        }

        public RecipeModel GetRecipe(int id)
        {
            List<RecipeSteps> steps = GetSteps(id);
            List<RecipeIngredientsModel> ingredients = GetIngredients(id);
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM recipes WHERE recipe_id = @recipe_id";

            var param = command.CreateParameter();
            param.ParameterName = "recipe_id";
            param.Value = id;

            command.Parameters.Add(param);
            using var reader = command.ExecuteReader();

            reader.Read();
            return RecipeModelFromData(reader, steps, ingredients);

        }
        private List<RecipeSteps> GetSteps(int id)
        {
            List<RecipeSteps> steps = new List<RecipeSteps>();
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM steps WHERE recipe_id = @recipe_id";

            var param = command.CreateParameter();
            param.ParameterName = "recipe_id";
            param.Value = id;

            command.Parameters.Add(param);
            using var reader = command.ExecuteReader();

            
            while (reader.Read())
            {
                steps.Add(ToSteps(reader));
            }
            return steps;
        }

        private List<RecipeIngredientsModel> GetIngredients(int id) 
        {
            List<RecipeIngredientsModel> ingredientsModels = new List<RecipeIngredientsModel>();
            using var command = _connection.CreateCommand();
            command.CommandText =   $"SELECT ingredients.ingredient_name, measurement_quantities.quantity_amount, measurement_units.measurement_unit_text " +
                                    "FROM recipe_ingredients " +
                                    "JOIN ingredients on ingredients.ingredient_id = recipe_ingredients.ingredient_id " +
                                    "JOIN measurement_quantities on recipe_ingredients.measurement_quantity_id = measurement_quantities.measurement_quantity_id " +
                                    "JOIN measurement_units on recipe_ingredients.measurement_unit_id = measurement_units.measurement_unit_id " +
                                    $"WHERE recipe_ingredients.recipe_id = {id};";

            var param = command.CreateParameter();
            param.ParameterName = "recipe_id";
            param.Value = id;

            command.Parameters.Add(param);
            using var reader = command.ExecuteReader();


            while (reader.Read())
            {
                ingredientsModels.Add(ToIngredient(reader));
            }
            return ingredientsModels;
                        
        }

        public List<RecipeModel> GetRecipes()
        {
            List<RecipeModel> recipes = new List<RecipeModel>();
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM recipes ";
                        
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                RecipeModel recipe = new RecipeModel
                {
                    RecipeID = (int)reader["recipe_id"],
                    RecipeName = (string)reader["recipe_name"],
                    Description = (string)reader["description"],
                };
                recipes.Add(recipe);
            }
            return recipes;
        }
    }
}
