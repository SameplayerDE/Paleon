using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class SaplingCmp : PlantCmp
    {
        public Color Color { get; private set; }
        public bool IsEvergreen { get; private set; }
        public bool IsWoody { get; private set; }
        public bool RemoveAfterHarvest { get; set; }
        public Item Fruit { get; private set; }
        public int FruitCount { get; private set; }

        private HarvestLabor harvestLabor;
        private ChopLabor chopLabor;

        private bool autoHarvest = false;
        public bool AutoHarvest
        {
            get { return autoHarvest; }
            set
            {
                if (autoHarvest == value)
                    return;

                autoHarvest = value;

                if(autoHarvest)
                {
                    Chop = false;
                }
                else
                {
                    if(harvestLabor != null)
                        harvestLabor.Cancel();
                    
                    harvestLabor = null;
                }
            }
        }

        private bool chop = false;
        public bool Chop
        {
            get { return chop; }
            set
            {
                if (chop == value)
                    return;

                chop = value;

                if(chop)
                {
                    chopLabor = new ChopLabor(Tile);
                    GameplayScene.WorldManager.LaborManager.Add(chopLabor);

                    AutoHarvest = false;
                }
                else
                {
                    if(chopLabor != null)
                        chopLabor.Cancel();
                    
                    chopLabor = null;
                }
            }
        }

        private Image fruitImage;

        public SaplingCmp(
            string name, 
            Tile tile, 
            int width, int height, 
            MyTexture texture, 
            Color color,  
            Item fruit,
            bool isEvergreen, 
            bool isWoody,
            bool isWalkable,
            List<Stage> stages) :
            base(name, tile, isWalkable, stages)
        {
            Color = color;
            Fruit = fruit;
            IsEvergreen = isEvergreen;
            IsWoody = isWoody;

            if (IsWoody)
            {
                MyTexture[] trunkTextures = new MyTexture[stages.Count];
                for (int i = 0; i < stages.Count; i++)
                    trunkTextures[i] = texture.GetSubtexture(i * width, height, width, height);
                textures.Add("Trunk", trunkTextures);


                Image trunkImage = new Image(RenderManager.Pixel, width, height);
                trunkImage.X = GameplayScene.TILE_SIZE / 2 - width / 2;
                trunkImage.Y = GameplayScene.TILE_SIZE - height;
                images.Add("Trunk", trunkImage);
            }

            MyTexture[] leavesTextures = new MyTexture[stages.Count];
            for (int i = 0; i < stages.Count; i++)
                leavesTextures[i] = texture.GetSubtexture(i * width, 0, width, height);
            textures.Add("Leaves", leavesTextures);

            Image leavesImage = new Image(RenderManager.Pixel, width, height);
            leavesImage.X = GameplayScene.TILE_SIZE / 2 - width / 2;
            leavesImage.Y = GameplayScene.TILE_SIZE - height;
            leavesImage.Color = color;
            images.Add("Leaves", leavesImage);

            if (Fruit != null)
            {
                fruitImage = new Image(RenderManager.Pixel, width, height);
                fruitImage.X = GameplayScene.TILE_SIZE / 2 - width / 2;
                fruitImage.Y = GameplayScene.TILE_SIZE - height;
                // Процесс созревания будет всегда находиться в правом верхне углу текстуры
                fruitImage.Texture = texture.GetSubtexture(texture.Width - width, 0, width, height);
            }
        }

        public void DoChop(SettlerCmp settler)
        {
            for(int resIndex = 0; resIndex < CurrentStage.Resources.Length; resIndex++)
            {
                int resCount = CurrentStage.Resources[resIndex].Item2;
                Item resource = CurrentStage.Resources[resIndex].Item1;

                for (int j = 0; j < resCount; j++)
                {
                    Tile.CreateItemContainer(resource);
                }
            }

            Remove();
        }

        public void DoHarvest(SettlerCmp settler)
        {
            for (int i = 0; i < FruitCount; i++)
                settler.Pathfinder.CurrentTile.CreateItemContainer(Fruit);

            if (!RemoveAfterHarvest)
            {
                // Reset growing progress to penultimate stage ending
                FruitCount = 0;
                GrowingProgress = stages[stages.Count - 2].Duration;
            }
            else
            {
                Remove();
            }

            harvestLabor = null;
        }

        public override void Begin()
        {
            base.Begin();

            if(Fruit != null)
                fruitImage.Entity = Entity;
        }

        public override void Render()
        {
            base.Render();

            if (Fruit != null && FruitCount > 0)
                fruitImage.Render();
        }

        protected override void UpdateGrowing()
        {
            base.UpdateGrowing();

            if (Fruit != null && GrowingProgress == 100)
            {
                FruitCount = 3;
                if (AutoHarvest == true && harvestLabor == null)
                {
                    harvestLabor = new HarvestLabor(Tile);
                    GameplayScene.WorldManager.LaborManager.Add(harvestLabor);
                }
            }
        }

    }
}
