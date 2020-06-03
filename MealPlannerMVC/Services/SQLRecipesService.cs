using MealPlannerMVC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
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
            
            return new RecipeIngredientsModel
            {
                IngredientName = (string)reader["ingredient_name"],
                MeasurementQuantity = (double)reader["quantity_amount"],
                MeasurementUnit = (string)reader["measurement_unit_text"]
                
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
            var recipe = RecipeModelFromData(reader, steps, ingredients);
            var jsonString = JsonSerializer.Serialize<RecipeModel>(recipe);
            Console.WriteLine(jsonString);

            return recipe;

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
            command.CommandText = $"SELECT ingredients.ingredient_name, measurement_quantities.quantity_amount, measurement_units.measurement_unit_text " +
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

        public List<RecipeIngredientsModel> GetALLIngredients()
        {
            List<RecipeIngredientsModel> ingredients = new List<RecipeIngredientsModel>();
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM ingredients ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                RecipeIngredientsModel ingredient = new RecipeIngredientsModel
                {
                    IngredientID = (int)reader["ingredient_id"],
                    IngredientName = (string)reader["ingredient_name"],


                };
                ingredients.Add(ingredient);
            }
            return ingredients;
        }

        public List<string> GetALLMeasurementUnits()
        {
            List<string> measurementUnits = new List<string>();
            using var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM measurement_units ";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {

                string measurementUnit = (string)reader["measurement_unit_text"];
                measurementUnits.Add(measurementUnit);
            }
            return measurementUnits;
        }


        public void AddRecipe(string jsonRecipe)
        {
            RecipeModel newRecipe = JsonSerializer.Deserialize<RecipeModel>(jsonRecipe);
            using var command = _connection.CreateCommand();
            
            var recipeNameParam = command.CreateParameter();
            recipeNameParam.ParameterName = "recipeName";
            recipeNameParam.Value = newRecipe.RecipeName;

            var descriptionParam = command.CreateParameter();
            descriptionParam.ParameterName = "recipeDescription";
            descriptionParam.Value = newRecipe.Description;

            var stepNumbersParam = command.CreateParameter();
            stepNumbersParam.ParameterName = "step_numbers";
            stepNumbersParam.Value = getStepnumbers(newRecipe.Steps);
            
            var stepsParam = command.CreateParameter();
            stepsParam.ParameterName = "steps";
            stepsParam.Value = getStepText(newRecipe.Steps); 

            var IngredientNamesParam = command.CreateParameter();
            IngredientNamesParam.ParameterName = "i_name";
            IngredientNamesParam.Value = getIngredientName(newRecipe.Ingredients);

            var IngredientQuantitiesParam = command.CreateParameter();
            IngredientQuantitiesParam.ParameterName = "i_quantity";
            IngredientQuantitiesParam.Value = getIngredientQuantity(newRecipe.Ingredients);

            var IngredientUnitsParam = command.CreateParameter();
            IngredientUnitsParam.ParameterName = "i_measurement_unit";
            IngredientUnitsParam.Value = getIngredientUnit(newRecipe.Ingredients);

                                            
            command.CommandText = "SELECT new_recipe(@recipeName, @recipeDescription,  @step_numbers, @steps, @i_name, @i_quantity, @i_measurement_unit)";

            command.Parameters.Add(recipeNameParam);
            command.Parameters.Add(descriptionParam);
            command.Parameters.Add(stepNumbersParam);
            command.Parameters.Add(stepsParam);
            command.Parameters.Add(IngredientNamesParam);
            command.Parameters.Add(IngredientQuantitiesParam);
            command.Parameters.Add(IngredientUnitsParam);
                                 
            command.ExecuteNonQuery();
        }
        private int[] getStepnumbers(List<RecipeSteps> recipeSteps)
        {
            
            List<int> stepnumbers = new List<int>();
            foreach (var item in recipeSteps)
            {

                stepnumbers.Add(item.StepNumber);
            }
            int[] resultArray = stepnumbers.ToArray();

            return resultArray;
        }
        private string[] getStepText(List<RecipeSteps> recipeSteps)
        {

            List<string> steptexts = new List<string>();
            foreach (var item in recipeSteps)
            {

                steptexts.Add(item.StepText);
            }
            string[] resultArray = steptexts.ToArray();

            return resultArray;
        }
        private string[] getIngredientName(List<RecipeIngredientsModel> ingredients)
        {

            List<string> ingredientNames = new List<string>();
            foreach (var item in ingredients)
            {

                ingredientNames.Add(item.IngredientName);
            }
            string[] resultArray = ingredientNames.ToArray();

            return resultArray;
        }
        private string[] getIngredientUnit(List<RecipeIngredientsModel> ingredients)
        {

            List<string> ingredientUnit = new List<string>();
            foreach (var item in ingredients)
            {

                ingredientUnit.Add(item.MeasurementUnit);
            }
            string[] resultArray = ingredientUnit.ToArray();

            return resultArray;
        }
        private double[] getIngredientQuantity(List<RecipeIngredientsModel> ingredients)
        {

            List<double> ingredientQuantity = new List<double>();
            foreach (var item in ingredients)
            {

                ingredientQuantity.Add(item.MeasurementQuantity);
            }
            double[] resultArray = ingredientQuantity.ToArray();

            return resultArray;
        }
    }
}
