using System;
using System.Collections.Generic;
using System.IO;

namespace MealPlanner.Model
{
    /// <summary>
    /// Implementation of ICook. 
    /// </summary>
    public class Cook : ICook
    {
        private readonly Pantry pantry;
        private readonly List<IRecipe> recipeBook;

        public Cook(Pantry pantry)
        {
            this.pantry = pantry;
            recipeBook = new List<IRecipe>();
        }

        /// <summary>
        /// returns a read only list of loaded recipes.
        /// </summary>
        public IEnumerable<IRecipe> RecipeBook => recipeBook;

        /// <summary>
        /// Loads recipes from the files.
        /// Must parse the name, success rate, needed ingredients and
        /// necessary quantities.
        /// </summary>
        /// <param name="recipeFiles">Array of file paths</param>
        public void LoadRecipeFiles(string[] recipeFiles)
        {
            foreach (string filePath in recipeFiles)
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    // Read and parse the first line: recipe name and success rate
                    string headerLine = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(headerLine))
                        throw new InvalidDataException($"Recipe file '{filePath}' missing header line.");

                    string[] data = headerLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (data.Length < 2)
                        throw new InvalidDataException($"Recipe header in '{filePath}' must contain name and success rate.");

                    string recipeName = data[0];
                    if (!double.TryParse(data[1], out double successRate))
                        throw new InvalidDataException($"Invalid success rate in recipe '{recipeName}' in file '{filePath}'.");

                    Dictionary<IIngredient, int> ingredients = new Dictionary<IIngredient, int>();

                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string[] ingData = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (ingData.Length < 2)
                            throw new InvalidDataException($"Malformed ingredient line in '{filePath}': '{line}'");

                        string ingredientName = ingData[0];
                        if (!int.TryParse(ingData[1], out int quantity))
                            throw new InvalidDataException($"Invalid quantity for ingredient '{ingredientName}' in '{filePath}'.");

                        IIngredient ing = pantry.GetIngredient(ingredientName);
                        if (ing == null)
                            throw new InvalidDataException($"Ingredient '{ingredientName}' not found in pantry when loading '{filePath}'.");

                        ingredients.Add(ing, quantity);
                    }

                    Recipe newRecipe = new Recipe(recipeName, successRate, ingredients);
                    recipeBook.Add(newRecipe);
                }
            }

            recipeBook.Sort();
        }


                  

        /// <summary>
        /// Attempts to cook a meal from a given recipe. Consumes pantry 
        /// ingredients and returns the result message.
        /// </summary>
        /// <param name="recipeName">Name of the recipe to cook</param>
        /// <returns>A message indicating success, failure, or error</returns>
        public string CookMeal(string recipeName)
        {
            IRecipe selected = null;

            for (int i = 0; i < recipeBook.Count; i++)
            {
                if (recipeBook[i].Name.Equals(recipeName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    selected = recipeBook[i];
                    break;
                }
            }

            if (selected == null)
                return "Recipe not found.";

            foreach (KeyValuePair<IIngredient, int> needed in selected.IngredientsNeeded)
            {
                IIngredient ingredient = needed.Key;
                int need = needed.Value;
                int have = pantry.GetQuantity(ingredient);
                if (have < need)
                {
                    if (have == 0)
                        return "Missing ingredient: " + ingredient.Name;

                    return "Not enough " + ingredient.Name +
                           " (need " + need + ", have " + have + ")";
                }
            }

            foreach (KeyValuePair<IIngredient, int> needed in selected.IngredientsNeeded)
                if (!pantry.ConsumeIngredient(needed.Key, needed.Value))
                    return "Not enough ingredients";

            Random rng = new Random();
            if (rng.NextDouble() < selected.SuccessRate)
                return "Cooking '" + selected.Name + "' succeeded!";
            else
                return "Cooking '" + selected.Name + "' failed. Ingredients burned...";

        }
    }
}
    
