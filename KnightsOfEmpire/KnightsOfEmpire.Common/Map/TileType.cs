using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Map
{
    [Serializable]
    public enum TileType
    {
        Walkable = 0,
        NonWalkable = 1, // can't walk, but vision is not blocked (e.x. water tiles)
        Wall = 2,
        Building = 3 // in case if buildings need it
    }
}
