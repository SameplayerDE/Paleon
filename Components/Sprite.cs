using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class Sprite : Image
    {

        public bool Animate;

        private Dictionary<string, Animation> animations;
        public Animation CurrentAnimation { get; private set; }
        public string CurrentAnimationKey { get; private set; }

        public Sprite(int width, int height) : base(null, width, height, true)
        {
            animations = new Dictionary<string, Animation>(StringComparer.OrdinalIgnoreCase);
            CurrentAnimationKey = "";
            Animate = true;
        }

        public void Add(string id, Animation animation)
        {
            animations.Add(id, animation);
        }

        public void Play(string id)
        {
            if (CurrentAnimationKey == id)
                return;

            CurrentAnimationKey = id;
            CurrentAnimation = animations[CurrentAnimationKey];
        }

        public void Reset()
        {
            CurrentAnimation?.Reset();
        }

        public override void Update()
        {
            if(Animate && CurrentAnimation != null)
            {
                CurrentAnimation.Update();
                Texture = CurrentAnimation.Frames[CurrentAnimation.CurrentFrame];
            }
        }

    }
}
