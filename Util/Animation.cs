using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Paleon
{
    public class Animation
    {
        public MyTexture[] Frames;
        private int framesPerSecond;
        private Timer timer;

        public bool IsLastFrame
        {
            get { return CurrentFrame == Frames.Length - 1; }
        }

        public int FramesPerSecond
        {
            get { return framesPerSecond; }
            set
            {
                framesPerSecond = MathHelper.Clamp(value, 1, 60);
            }
        }

        public int CurrentFrame
        {
            get; set;
        }

        public int DefaultFrame
        {
            get; private set;
        }

        public Animation(MyTexture texture, int frameCount, int defaultFrame, int frameWidth, int frameHeight, int xOffset, int yOffset, int speed = 5)
        {
            Frames = new MyTexture[frameCount];
            DefaultFrame = defaultFrame;

            for (int i = 0; i < frameCount; i++)
            {
                Frames[i] = texture.GetSubtexture(xOffset + (frameWidth * i), yOffset, frameWidth, frameHeight);
            }
            
            FramesPerSecond = speed;
            timer = new Timer();
            Reset();
        }

        public void Update()
        {
            if(timer.GetTime() >= (1.0f / FramesPerSecond))
            {
                CurrentFrame = (CurrentFrame + 1) % Frames.Length;
                timer.Reset();
            }
        }

        public void Reset()
        {
            CurrentFrame = DefaultFrame;
            timer.Reset();
        }
    }
}
