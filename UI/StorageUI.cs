using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class StorageUI : UI
    {

        private PanelUI panel;
        private PixelTextUI nameText;

        private StorageBuildingCmp storage;

        private ElementUI[] elements;

        public StorageUI()
        {
            panel = new PanelUI();
            panel.Color = UIRenderer.DefaultColor;

            panel.InnerWidth = 256;
            panel.InnerHeight = 32 * 10 + 5 * (10 - 1);
            panel.X = Engine.Width - panel.Width - 5;
            panel.Y = Engine.Height - panel.Height - 5;

            nameText = new PixelTextUI(RenderManager.PixelFont, "", Color.White);
            nameText.X = panel.InnerX;
            nameText.Y = panel.InnerY;

            elements = new ElementUI[8];

            for (int i = 0; i < elements.Length; ++i)
            {
                elements[i] = new ElementUI();
            }

            UpdatePositions();
        }

        private void UpdatePositions()
        {
            for (int i = 0; i < elements.Length; i++)
            {
                ElementUI element = elements[i];
                element.X = panel.InnerX;
                element.Y = (nameText.Height + 5) + (panel.InnerY + 5) + (i * (ElementUI.HEIGHT + 5));
            }
        }

        public override void Open(object obj)
        {
            StorageBuildingCmp storage = obj as StorageBuildingCmp;
            this.storage = storage;

            nameText.Text = storage.Name;

            UpdateSlots();
        }

        private void UpdateSlots()
        {
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].Visible = false;
            }

            int count = 0;
            foreach(var entry in storage.Slots)
            {
                ElementUI element = elements[count];
                element.SetItem(entry.Key, entry.Value);
                element.Visible = true;
                count += 1;
            }
        }

        public override void Close()
        {
        }

        public override void Update()
        {
        }

        public override void Render()
        {
            panel.Render();
            nameText.Render();
            for (int i = 0; i < elements.Length; ++i)
            {
                elements[i].Render();
            }
        }

        private class ElementUI
        {
            public const int WIDTH = 256;
            public const int HEIGHT = 32;

            public Item Item { get; private set; }

            public bool Visible;

            public ImageUI ItemImage { get; private set; }
            public PixelTextUI ItemNameText { get; private set; }
            public PixelTextUI ItemWeightText { get; private set; }

            private int x;
            public int X
            {
                get { return x; }
                set
                {
                    x = value;
                    ItemImage.X = value;
                    ItemNameText.X = ItemImage.X + ItemImage.Width + 5;
                    ItemWeightText.X = ItemNameText.X + ItemNameText.Width + 5;
                }
            }

            private int y;
            public int Y
            {
                get { return y; }
                set
                {
                    y = value;
                    ItemImage.Y = value;
                    ItemNameText.Y = value;
                    ItemWeightText.Y = value;
                }
            }

            public ElementUI()
            {
                ItemImage = new ImageUI(RenderManager.Pixel, 32, 32);
                ItemNameText = new PixelTextUI(RenderManager.PixelFont, "", Color.White);
                ItemWeightText = new PixelTextUI(RenderManager.PixelFont, "", Color.White);
            }

            public void SetItem(Item item, int count)
            {
                Item = item;
                ItemImage.Texture = Item.Texture;
                ItemNameText.Text = item.Name;

                ItemWeightText.Text = " [" + count + "]";

                ItemWeightText.X = ItemNameText.X + ItemNameText.Width + 5;
            }

            public void Render()
            {
                if (Visible)
                {
                    ItemImage.Render();
                    ItemNameText.RenderShadow(2);
                    ItemWeightText.RenderShadow(2);
                }
            }
        }
    }
}
