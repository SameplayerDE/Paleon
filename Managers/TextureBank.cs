using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paleon
{
    public static class TextureBank
    {

        public static MyTexture UITexture { get; private set; }
        public static MyTexture GroundTexture { get; private set; }
        public static MyTexture BlockTexture { get; private set; }
        public static MyTexture PanelTopLeft { get; private set; }
        public static MyTexture PanelTop { get; private set; }
        public static MyTexture PanelTopRight { get; private set; }
        public static MyTexture PanelLeft { get; private set; }
        public static MyTexture PanelMiddle { get; private set; }
        public static MyTexture PanelRight { get; private set; }
        public static MyTexture PanelBottomLeft { get; private set; }
        public static MyTexture PanelBottom { get; private set; }
        public static MyTexture PanelBottomRight { get; private set; }
        public static MyTexture BackTexture { get; private set; }
        public static MyTexture ForwardTexture { get; private set; }
        public static MyTexture RepeatTexture { get; private set; }
        public static MyTexture SmallButtonTexture { get; private set; }
        public static MyTexture ButtonTexture { get; private set; }
        public static MyTexture ElementBackgroundTexture { get; private set; }
        public static MyTexture MiddleButtonTexture { get; private set; }
        public static MyTexture BigButtonTexture { get; private set; }
        public static MyTexture FirstSpeedTexture { get; private set; }
        public static MyTexture SecondSpeedTexture { get; private set; }
        public static MyTexture ThirdSpeedTexture { get; private set; }
        public static MyTexture PauseTexture { get; private set; }

        public static Tileset GroundTileset { get; private set; }
        public static Tileset GroundTopTileset { get; private set; }
        public static Tileset BlockTileset { get; private set; }
        public static Tileset UiTileset { get; private set; }

        public static void Initialize()
        {
            UITexture = ResourceManager.GetTexture("ui");
            GroundTexture = ResourceManager.GetTexture("tileset");
            BlockTexture = ResourceManager.GetTexture("block_tileset");

            PanelTopLeft = UITexture.GetSubtexture(0, 48, 4, 4);
            PanelTop = UITexture.GetSubtexture(4, 48, 4, 4);
            PanelTopRight = UITexture.GetSubtexture(8, 48, 4, 4);

            PanelLeft = UITexture.GetSubtexture(0, 52, 4, 4);
            PanelMiddle = UITexture.GetSubtexture(4, 52, 4, 4);
            PanelRight = UITexture.GetSubtexture(8, 52, 4, 4);

            PanelBottomLeft = UITexture.GetSubtexture(0, 56, 4, 4);
            PanelBottom = UITexture.GetSubtexture(4, 56, 4, 4);
            PanelBottomRight = UITexture.GetSubtexture(8, 56, 4, 4);

            BackTexture = UITexture.GetSubtexture(176, 16, 16, 16);
            ForwardTexture = UITexture.GetSubtexture(192, 16, 16, 16);
            RepeatTexture = UITexture.GetSubtexture(208, 16, 16, 16);

            SmallButtonTexture = UITexture.GetSubtexture(16, 48, 16, 16);
            ButtonTexture = UITexture.GetSubtexture(0, 64, 24, 24);
            BigButtonTexture = UITexture.GetSubtexture(32, 64, 40, 40);
            MiddleButtonTexture = UITexture.GetSubtexture(0, 88, 18, 18);
            ElementBackgroundTexture = UITexture.GetSubtexture(0, 112, 128, 16);

            GroundTileset = new Tileset(GroundTexture, 16, 16);
            GroundTopTileset = new Tileset(ResourceManager.GetTexture("ground_top"), 16, 16);
            BlockTileset = new Tileset(BlockTexture, 16, 16);
            UiTileset = new Tileset(UITexture, 16, 16);
        }

    }
}
