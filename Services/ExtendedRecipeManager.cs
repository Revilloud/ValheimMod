using System.Collections.Generic;
using Jotunn.Utils;
using Timber.Models;

namespace Timber.Services
{
    internal class ExtendedRecipeManager
    {
        public static List<ExtendedRecipe> LoadRecipesFromJson(string recipesPath)
        {
            var json = AssetUtils.LoadText(recipesPath);
            return SimpleJson.SimpleJson.DeserializeObject<List<ExtendedRecipe>>(json);
        }
    }
}