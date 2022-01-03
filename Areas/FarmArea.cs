using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Paleon
{
    public class FarmArea : Area
    {
        private Item plant;
        public Item Plant 
        {
            get { return plant; } 
            set
            {
                if (plant == value)
                    return;

                plant = value;

                //TODO: if there was another plant, need to destroy old
                
                for (int i = 0; i < Tiles.Count; ++i)
                {
                    PlantLabor labor = new PlantLabor(Tiles[i], plant);
                    plantLabors.Add(labor);
                    GameplayScene.WorldManager.LaborManager.Add(labor);
                }
            } 
        }

        private List<PlowLabor> plowingLabors;
        private List<PlantLabor> plantLabors;

        public FarmArea() : base("Farm", Color.Yellow)
        {
            plowingLabors = new List<PlowLabor>();
            plantLabors = new List<PlantLabor>();
        }

        public override void AddTiles(List<Tile> tiles)
        {
            base.AddTiles(tiles);

            // Создаем работу по вспахиванию земли
            for (int i = 0; i < tiles.Count; i++)
            {
                Tile tile = tiles[i];

                PlowLabor labor = new PlowLabor(tile);
                plowingLabors.Add(labor);
                GameplayScene.WorldManager.LaborManager.Add(labor);
            }
            
        }

    }
}
