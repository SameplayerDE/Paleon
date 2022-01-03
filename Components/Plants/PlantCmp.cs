using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class Stage
    {
        public int Index { get; private set; } 
        public int Duration { get; private set; }

        public Tuple<Item, int>[] Resources { get; private set; }

        public Stage(int index, int duration, params Tuple<Item, int>[] resources)
        {
            Index = index;
            Duration = duration;

            Resources = resources;
        }
    }

    public abstract class PlantCmp : Component
    {
        public string Name { get; private set; }

        public Tile Tile { get; private set; }
        public bool IsWalkable { get; private set; }

        public Labor Labor { get; protected set; }

        private Stage currentStage;
        public Stage CurrentStage
        { 
            get { return currentStage; }
            private set
            {
                if (currentStage != null && currentStage.Index == value.Index)
                    return;

                currentStage = value;

                foreach (var entry in images)
                {
                    entry.Value.Texture = textures[entry.Key][currentStage.Index];
                }
            }
        }

        protected List<Stage> stages;

        protected Dictionary<string, Image> images;
        protected Dictionary<string, MyTexture[]> textures;

        public float GrowingProgress { get; protected set; } = 0;
        private float growingModificator = 5.0f;
        private Timer timer;

        public PlantCmp(string name, Tile tile, bool isWalkable, List<Stage> stages) : base(true, true)
        {
            Name = name;
            Tile = tile;
            IsWalkable = isWalkable;

            this.stages = stages;

            images = new Dictionary<string, Image>();
            textures = new Dictionary<string, MyTexture[]>();
        }

        public override void Begin()
        {
            Tile.IsWalkable = IsWalkable;

            Entity.X = Tile.X * GameplayScene.TILE_SIZE;
            Entity.Y = Tile.Y * GameplayScene.TILE_SIZE;

            foreach (var entry in images)
            {
                entry.Value.Entity = Entity;
            }

            timer = new Timer();

            CurrentStage = stages[0];
        }

        public override void Update()
        {
            if(timer.GetTime() > 1.0f)
            {
                timer.Reset();

                UpdateGrowing();
            }
        }

        protected virtual void UpdateGrowing()
        {
            GrowingProgress = MathHelper.Clamp(GrowingProgress + growingModificator, 0, 100);

            if (GrowingProgress >= CurrentStage.Duration)
            {
                // Проверяем есть ли следующая стадия роста, если есть, то переходим на нее
                int indexOfNextStage = stages.IndexOf(CurrentStage) + 1;
                if (indexOfNextStage < stages.Count)
                    CurrentStage = stages[indexOfNextStage];
            }
        }

        public override void Render()
        {
            foreach(KeyValuePair<string, Image> entry in images)
            {
                entry.Value.Render();
            }
        }

        public void Remove()
        {
            Tile.RemoveEntity();
            Entity.RemoveSelf();
        }

    }
}
