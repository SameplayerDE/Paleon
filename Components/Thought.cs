using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{

    public enum ThoughtsFlags
    {
        SLEEP = 1,
        EAT = 2,
    }

    public class Thought : Component
    {

        private Sprite sprite;

        private int showTime = 5;
        private Timer timer;

        private ThoughtsFlags currentThought;

        public override Entity Entity 
        {
            get { return base.Entity; }
            internal set 
            { 
                base.Entity = value;
                sprite.Entity = value;
            }
        }

        public int X
        {
            set { sprite.X = value; }
        }

        public int Y
        {
            set { sprite.Y = value; }
        }

        public Thought() : base(false, false)
        {
            sprite = new Sprite(16, 16);
            sprite.Add("Begin", new Animation(ResourceManager.GetTexture("emotest"), 5, 0, 16, 16, 0, 0, 3));
            sprite.Add("Sleep", new Animation(ResourceManager.GetTexture("emotest"), 3, 0, 16, 16, 0, 16, 2));
            sprite.Add("Eat", new Animation(ResourceManager.GetTexture("emotest"), 3, 0, 16, 16, 0, 32, 2));

            timer = new Timer();
        }

        public override void Update()
        {
            if (Active)
            {
                sprite.Update();

                if (timer.GetTime() < showTime)
                {
                    if (sprite.CurrentAnimationKey == "Begin" && sprite.CurrentAnimation.IsLastFrame)
                    {
                        switch (currentThought)
                        {
                            case ThoughtsFlags.SLEEP:
                                sprite.Play("Sleep");
                                break;
                            case ThoughtsFlags.EAT:
                                sprite.Play("Eat");
                                break;
                        }
                    }
                }
                else
                {
                    timer.Reset();
                    Active = false;
                    Visible = false;
                }
            }
        }

        public override void Render()
        {
            if(Visible)
            {
                sprite.Render();
            }
        }

        public void Show(ThoughtsFlags thought)
        {
            currentThought = thought;
            Active = true;
            Visible = true;
            sprite.Play("Begin");
        }

    }
}
