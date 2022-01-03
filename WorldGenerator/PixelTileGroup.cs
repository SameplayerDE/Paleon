using System.Collections.Generic;

public enum TileGroupType
{
	Water, 
	Land
}

namespace Paleon
{

    public class PixelTileGroup
    {

        public TileGroupType Type;
        public List<PixelTile> Tiles;

        public PixelTileGroup()
        {
            Tiles = new List<PixelTile>();
        }
    }
}
