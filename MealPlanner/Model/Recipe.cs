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
        public IReadOnlyDictionary<IIngredient, int> RequiredIngredient { get; }
        public double SuccessRate { get; }
        
        public Recipe(string name, double successRate, Dictionary<IIngredient, int> ingredient)
        {
            Name = name;
            SuccessRate = successRate;
            RequiredIngredients = ingredients;
        }
        
        public int CompareTo(IRecipe obj)
        {
            if (obj == null) return 1;
            return Name.CompareTo(obj.Name);
        }
        
    }
}