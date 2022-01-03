using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public class Settler : Entity
    {

        public Settler(Scene scene, Tile[,] tiles, string name, bool male) : base(scene)
        {
            MyTexture texture = ResourceManager.GetTexture("settler");

            Sprite sprite = new Sprite(16, 24);

            if (male)
            {
                sprite.Add("Walk", new Animation(texture, 8, 0, 16, 24, 0, 0, 7));
                sprite.Add("Idle", new Animation(texture, 2, 0, 16, 24, 0, 96, 2));
                sprite.Add("Sleep", new Animation(texture, 2, 0, 16, 24, 32, 96, 2));
                sprite.Add("Eat", new Animation(texture, 2, 0, 16, 24, 64, 96, 1));
            }
            else
            {
                sprite.Add("Walk", new Animation(texture, 8, 0, 16, 24, 0, 48, 7));
                sprite.Add("Idle", new Animation(texture, 2, 0, 16, 24, 0, 144, 2));
                sprite.Add("Sleep", new Animation(texture, 2, 0, 16, 24, 32, 144, 2));
                sprite.Add("Eat", new Animation(texture, 2, 0, 16, 24, 64, 144, 1));
            }

            sprite.Y = -8;

            Sprite clothingSprite = new Sprite(16, 24);
            clothingSprite.Add("Walk", new Animation(texture, 8, 0, 16, 24, 0, 24, 7));
            clothingSprite.Add("Idle", new Animation(texture, 2, 0, 16, 24, 0, 120, 2));
            clothingSprite.Add("Sleep", new Animation(texture, 2, 0, 16, 24, 32, 120, 2));
            clothingSprite.Add("Eat", new Animation(texture, 2, 0, 16, 24, 0, 120, 1));
            clothingSprite.Visible = false;

            clothingSprite.Y = -8;

            Image shadow = new Image(texture.GetSubtexture(112, 168, 16, 8));
            shadow.Y = 11;

            Add(new PathfinderCmp(tiles));
            Add(new InventoryCmp());
            Add(shadow);
            Add(new SettlerCmp(sprite, clothingSprite, TextureBank.UITexture.GetSubtexture(48, 32, 16, 16), name));
            Add(new SelectableCmp(0, -8, 16, 24));
        }

    }
}
