using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Paleon
{
    public enum MovementState
    {
        Running,
        Success,
        Fail,
        Completion
    }

    public class SettlerCmp : Component
    {
        public MyTexture Avatar { get; private set; }
        public string Name { get; private set; }

        public HutCmp Hut { get; set; }

        public SliderUI Slider { get; private set; }

        private Sprite sprite;

        private Thought thought;

        private SleepLabor sleepLabor;
        private EatLabor eatLabor;
        private DrinkLabor drinkLabor;

        public PathfinderCmp Pathfinder { get; private set; }

        public Labor Labor { get; set; }
        private Dictionary<LaborType, bool> laborsPriorities;

        public Stats Stats { get; private set; }
        private int thirstLimit = 40;
        private int hungerLimit = 60;

        public InventoryCmp Inventory { get; private set; }
        private Image toolImage;
        private Image cargoImage;
        private Sprite clothingSprite;

        private float defaultMovementSpeed = 3;
        private float movementSpeed;
        public float MovementSpeed
        {
            get { return movementSpeed; }
            set
            {
                movementSpeed = value;
                Pathfinder.MovementSpeed = value;
            }
        }

        public SettlerCmp(Sprite sprite, Sprite clothingSprite, MyTexture avatar, string name) : base(true, true)
        {
            this.sprite = sprite;
            this.clothingSprite = clothingSprite;
            Avatar = avatar;
            Name = name;
        }

        public void PlayEatAnimation()
        {
            sprite.Play("Eat");
            clothingSprite.Play("Eat");
        }


        public void PlayIdleAnimation()
        {
            sprite.Play("Idle");
            clothingSprite.Play("Idle");

            CheckDirection();
        }

        public void PlaySleepAnimation()
        {
            sprite.Play("Sleep");
            clothingSprite.Play("Sleep");

            CheckDirection();
        }

        public void PlayWalkAnimation()
        {
            sprite.Play("Walk");
            clothingSprite.Play("Walk");

            CheckDirection();
        }

        public override void Awake()
        {
            cargoImage = new Image(RenderManager.Pixel, 16, 16);
            cargoImage.Entity = Entity;
            cargoImage.Visible = false;

            toolImage = new Image(RenderManager.Pixel, 16, 16);
            toolImage.Entity = Entity;
            toolImage.Visible = false;

            Pathfinder = Entity.Get<PathfinderCmp>();
        }

        public override void Begin()
        {
            InitializeLaborsPriorities();

            PlayIdleAnimation();
            sprite.Entity = Entity;
            clothingSprite.Entity = Entity;

            MovementSpeed = defaultMovementSpeed;

            Slider = new SliderUI(16, 4, Color.Black, Color.Orange);
            Slider.Active = false;

            Stats = new Stats(100, 100)
            {
                HungerModificator = -0.08f,
                ThirstModificator = -0.14f
            };

            Inventory = Entity.Get<InventoryCmp>();
            Inventory.RegisterOnCargoChangedCallback(LoadCargo);
            Inventory.RegisterOnToolChangedCallback(EquipTool);
            Inventory.RegisterOnClothingChangedCallback(PutOnClothing);

            // TODO: remove
            //Inventory.Clothing = ItemDatabase.FUR_CLOTHING;

            thought = new Thought();
            thought.Entity = Entity;
            thought.Y = -24;
        }

        public override void Update()
        {
            Stats.Update();

            sprite.Update();
            clothingSprite.Update();

            thought.Update();
        }

        // If not rendering - seems like component is invisible
        public override void Render()
        {
            if (Slider.Active)
            {
                Slider.Position = new Vector2(Entity.Position.X, Entity.Position.Y - 15);
                Slider.Render();
            }

            // Сперва отображаем груз
            if(cargoImage.Visible)
                cargoImage.Render();

            // Далее поселенца
            sprite.Render();

            // Одежду
            if(clothingSprite.Visible)
                clothingSprite.Render();

            // И только потом инструмент
            if (toolImage.Visible)
                toolImage.Render();

            thought.Render();
        }

        private void CheckDirection()
        {
            if (Pathfinder.Direction == Direction.LEFT)
            {
                sprite.FlipX = false;
                clothingSprite.FlipX = false;

                cargoImage.Y = -10;
                cargoImage.X = 2;

                toolImage.Y = -6;
                toolImage.X = -8;
                toolImage.FlipX = false;
            }
            else if (Pathfinder.Direction == Direction.RIGHT)
            {
                sprite.FlipX = true;
                clothingSprite.FlipX = true;

                cargoImage.Y = -10;
                cargoImage.X = -2;

                toolImage.Y = -6;
                toolImage.X = 8;
                toolImage.FlipX = true;
            }
        }

        private void EquipTool(Item item)
        {
            if(item != null)
            {
                toolImage.Texture = item.Texture;
                toolImage.Visible = true;
            }
            else
            {
                toolImage.Visible = false;
            }
        }

        private void PutOnClothing(Item item)
        {
            if (item != null)
            {
                clothingSprite.Visible = true;
            }
            else
            {
                clothingSprite.Visible = false;
            }
        }

        private void LoadCargo(Item item)
        {
            if (item != null)
            {
                cargoImage.Texture = item.Texture;
                cargoImage.Visible = true;
            }
            else
            {
                cargoImage.Visible = false;
            }
        }

        // Enable or disable work for this settler
        public void PermitLabor(LaborType labor, bool value)
        {
            laborsPriorities[labor] = value;
        }

        private int currentLaborIteration = 0;
        private LaborType currentLaborType = LaborType.Miner;

        public Labor LookForLabor(Dictionary<LaborType, List<Labor>> laborsByType)
        {
            // Eat labor
            if (Stats.CurrentHunger <= hungerLimit)
            {
                if (eatLabor == null)
                {
                    eatLabor = new EatLabor();
                    eatLabor.Repeat = true;
                }

                if (eatLabor.Check(this))
                {
                    return eatLabor;
                }
            }

            // Drink labor
            if (Stats.CurrentThirst <= thirstLimit)
            {
                if(drinkLabor == null)
                {
                    drinkLabor = new DrinkLabor(GameplayScene.WorldManager.WaterAreas);
                    drinkLabor.Repeat = true;
                }

                if (drinkLabor.Check(this))
                {
                    return drinkLabor;
                }
            }
            
            if (GameplayScene.WorldManager.WorldTimer.IsNight)
            {
                if (sleepLabor == null)
                {
                    sleepLabor = new SleepLabor();
                    sleepLabor.Repeat = true;
                }

                if (sleepLabor.Check(this))
                {
                    thought.Show(ThoughtsFlags.SLEEP);
                    return sleepLabor;
                }
            }

            // Check if labor is allowed and any labors exists else got to check next labor type
            if (laborsPriorities[currentLaborType] == true && laborsByType[currentLaborType].Count > 0)
            {
                List<Labor> labors = laborsByType[currentLaborType];

                if (currentLaborIteration >= labors.Count)
                    currentLaborIteration = 0;

                Labor labor = labors[currentLaborIteration];
                if (labor.Owner == null && !labor.IsCompleted && labor.Check(this))
                {
                    currentLaborIteration = 0;

                    return labor;
                }

                currentLaborIteration++;
                if (currentLaborIteration >= labors.Count)
                {
                    currentLaborIteration = 0;
                    NextLaborType();
                }
            }
            else
            {
                NextLaborType();
            }

            return null;
        }

        // Переход к следующему типу работы
        private void NextLaborType()
        {
            currentLaborType += 1;
            if (currentLaborType == LaborType.NONE)
                currentLaborType = 0;
        }

        private void InitializeLaborsPriorities()
        {
            // Инициализация приоритета работ
            laborsPriorities = new Dictionary<LaborType, bool>();

            foreach (LaborType laborType in Enum.GetValues(typeof(LaborType)))
            {
                laborsPriorities.Add(laborType, true);
            }
        }

    }
}
