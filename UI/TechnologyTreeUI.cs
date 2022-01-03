using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class TechnologyTreeUI : UI
    {

        private struct TechCost
        {
            public string Name { get; private set; }
            public int Cost { get; private set; }

            public TechCost(string name, int cost)
            {
                Name = name;
                Cost = cost;
            }
        }

        private PanelUI panel;

        private List<ButtonUI> buttons;
        private Dictionary<ButtonUI, TechCost> technologies;

        private int rows = 7;
        private int columns = 10;
        private int offset = 5;

        private int buttonSize = 48;

        public TechnologyTreeUI()
        {
            panel = new PanelUI();
            panel.InnerWidth = (columns * buttonSize) + (columns * offset) + offset;
            panel.InnerHeight = (rows * buttonSize);

            panel.X = Engine.Width / 2 - panel.Width / 2;
            panel.Y = Engine.Height / 2 - panel.Height / 2;

            buttons = new List<ButtonUI>();
            technologies = new Dictionary<ButtonUI, TechCost>();
        }

        public override void Open(object obj)
        {

        }

        public override void Update()
        {
            int x = MInput.Mouse.X;
            int y = MInput.Mouse.Y;

            if(panel.Intersects(x, y))
            {
                GameplayScene.MouseOnUI = true;

                for (int i = 0; i < buttons.Count; i++)
                {
                    ButtonUI button = buttons[i];
                    button.Update();

                    if (button.Hovered)
                    {
                        TechCost tech = technologies[button];
                        GameplayScene.UIRenderer.ShowPopUp(tech.Name + " *" + tech.Cost);
                    }
                }
            }


        }

        public override void Render()
        {
            panel.Render();

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].Render();
            }
        }

        public override void Close()
        {

        }
    
        public void AddTechnology(string name, int cost, MyTexture icon, int cellX, int cellY)
        {
            ButtonUI button = new ButtonUI(TextureBank.ButtonTexture, icon, buttonSize, buttonSize, 32, 32);
            button.DefaultColor = UIRenderer.DefaultColor;
            button.HoveredColor = UIRenderer.HoveredColor;
            button.SelectedColor = UIRenderer.SelectedColor;
            button.Selectable = true;
            buttons.Add(button);
            technologies.Add(button, new TechCost(name, cost));

            button.X = (panel.InnerX + cellX * buttonSize);
            button.Y = (panel.InnerY + cellY * buttonSize);
        }
    }
}
