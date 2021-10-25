using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Map
{
    public enum TileType
    {
        Walkable = 0,
        NonWalkable, // can't walk, but vision is not blocked (e.x. water tiles)
        Wall,
        Building // in case if buildings need it
    }
}
