using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public enum ItemType
    {
        AXE,
        PICKAXE,
        SICKLE,
        HARPOON,
        SPEAR,
        CLOTHING,
        FOOD,
        KNIFE,
        CONTAINER,
        NONE
    }


    public class Item
    {
        public int Id { get; private set; }
        public MyTexture Texture { get; private set; }
        public string Name { get; private set; }
        public int FoodValue { get; private set; }
        public int Damage { get; private set; }
        public ItemType ItemType {get; private set;}

        public Item(int id, MyTexture texture, string name, int foodValue, int damage, ItemType itemType)
        {
            Id = id;
            Texture = texture;
            Name = name;
            FoodValue = foodValue;
            Damage = damage;
            ItemType = itemType;
        }

        public void Eat(SettlerCmp settler)
        {
            settler.Stats.CurrentHunger += FoodValue;
        }

        public void Eat(AnimalCmp animalCmp)
        {
            animalCmp.Stats.CurrentHunger += FoodValue;
        }

    }
}
