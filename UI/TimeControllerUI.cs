using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Paleon
{
    public class TimeControllerUI : UI
    {

        private WorldTimer worldTimer;

        private ImageUI wheelBack;
        private ImageUI wheelFront;

        private PixelTextUI seasonText;

        private const int BUTTON_SIZE = 36;
        private const int OFFSET = 5;

        private Dictionary<ButtonUI, int> timeButtons;
        private List<ButtonUI> buttons;

        private ButtonUI lastSelectedButton;
        private ButtonUI lastHoveredButton;

        private Rectangle dest;

        public bool Paused { get; private set; }
        public int CurrentSpeed { get; private set; }

        public TimeControllerUI(WorldTimer worldTimer)
        {
            this.worldTimer = worldTimer;

            wheelBack = new ImageUI(TextureBank.UITexture.GetSubtexture(0, 16, 32, 32), 96, 96);
            wheelBack.CenterOrigin();
            wheelBack.X = OFFSET + 96 / 2;
            wheelBack.Y = OFFSET + 96 / 2;

            wheelFront = new ImageUI(TextureBank.UITexture.GetSubtexture(32, 16, 32, 16), 96, 48);
            wheelFront.X = OFFSET;

            seasonText = new PixelTextUI(RenderManager.PixelFont, "", Color.White);
            // При смене сезона сменяем текст сезона
            worldTimer.RegisterOnSeasonChangedCallback(ChangeSeasonText);

            timeButtons = new Dictionary<ButtonUI, int>();
            buttons = new List<ButtonUI>();

            AddButton(1, TextureBank.UITexture.GetSubtexture(64, 32, 16, 16));
            AddButton(2, TextureBank.UITexture.GetSubtexture(80, 32, 16, 16));
            AddButton(4, TextureBank.UITexture.GetSubtexture(96, 32, 16, 16));
            AddButton(0, TextureBank.UITexture.GetSubtexture(112, 32, 16, 16));

            dest = new Rectangle();

            UpdatePositions();

            SetTimeSpeed(buttons[0]);
        }

        private void AddButton(int speed, MyTexture icon)
        {
            ButtonUI button = new ButtonUI(TextureBank.MiddleButtonTexture, icon, BUTTON_SIZE, BUTTON_SIZE, 32, 32);
            button.DefaultColor = UIRenderer.DefaultColor;
            button.HoveredColor = UIRenderer.HoveredColor;
            button.SelectedColor = UIRenderer.SelectedColor;
            button.Selectable = true;

            button.AddOnPressedCallback(SetTimeSpeed);

            timeButtons.Add(button, speed);
            buttons.Add(button);
        }

        private void UpdatePositions()
        {
            int totalWidth = buttons.Count * BUTTON_SIZE + (buttons.Count - 1) * OFFSET;
            int totalHeight = BUTTON_SIZE;

            int startX = wheelFront.X +  wheelFront.Width + OFFSET;
            int startY = OFFSET;

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].X = startX + i * (BUTTON_SIZE + OFFSET);
                buttons[i].Y = startY;
            }

            dest.X = startX;
            dest.Y = startY;
            dest.Width = totalWidth;
            dest.Height = totalHeight;

            // Распологаем текст сезона под кнопками управления времени
            seasonText.X = startX;
            seasonText.Y = startY + totalHeight + OFFSET;
        }

        private void ChangeSeasonText(Season season)
        {
            seasonText.Text = Utils.EnumToString(season);
        }

        public override void Update()
        {
            wheelBack.Rotation = -MathHelper.ToRadians(worldTimer.CurrentTimeInDegrees);

            if (lastHoveredButton != null)
                lastHoveredButton.Hovered = false;

            if(Intersects(MInput.Mouse.X, MInput.Mouse.Y))
            {
                GameplayScene.MouseOnUI = true;

                for(int i = 0; i < buttons.Count; i++)
                {
                    ButtonUI button = buttons[i];

                    button.Update();

                    if (button.Hovered)
                        lastHoveredButton = button;
                }
            }

            if (MInput.Keyboard.Pressed(Keys.D1))
                SetTimeSpeed(buttons[0]);
            else if(MInput.Keyboard.Pressed(Keys.D2))
                SetTimeSpeed(buttons[1]);
            else if (MInput.Keyboard.Pressed(Keys.D3))
                SetTimeSpeed(buttons[2]);
            else if(MInput.Keyboard.Pressed(Keys.Space))
                SetTimeSpeed(buttons[3]);
        }

        public override void Render()
        {
            wheelBack.Render();
            wheelFront.Render();

            for (int i = 0; i < buttons.Count; i++)
                buttons[i].Render();

            seasonText.RenderShadow(2);
        }

        public override void Open(object obj)
        {
        }

        public override void Close()
        {
        }

        public bool Intersects(int x, int y)
        {
            return dest.Contains(new Point(x, y));
        }

        private void SetTimeSpeed(ButtonUI button)
        {
            if(lastSelectedButton != null)
                lastSelectedButton.Selected = false;

            button.Selected = true;
            lastSelectedButton = button;

            Engine.GameSpeed = timeButtons[button];
        }
    }
}
