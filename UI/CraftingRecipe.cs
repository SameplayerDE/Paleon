using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class CraftingRecipe
    {

        public Item[] Ingredients { get; private set; }
        public Item Result { get; private set; }

        public CraftingRecipe(Item result, params Item[] ingredients)
        {
            Result = result;
            Ingredients = ingredients;
        }

    }
}
