using System;
using System.Collections.Generic;

namespace MealPlanner.Model
{
    /// <summary>
    /// Implementation of Recipe. 
    /// </summary>
   public class Recipe : IRecipe
    {

        public string Name { get; }
        public IReadOnlyDictionary<IIngredient, int> IngredientsNeeded { get; }
        public double SuccessRate { get; }
        
        public Recipe(string name, double successRate, Dictionary<IIngredient, int> ingredients)
        {
            Name = name;
            SuccessRate = successRate;
            IngredientsNeeded = ingredients;
        }
        
        public int CompareTo(IRecipe obj)
        {
            if (obj == null) return 1;
            return Name.CompareTo(obj.Name);
        }
        
    }
}