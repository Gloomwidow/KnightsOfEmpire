using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameMap = KnightsOfEmpire.Common.Map.Map;

namespace KnightsOfEmpire.Common.Units.Groups
{
    public class UnitGroup
    {
        public float TargetX;
        public float TargetY;

        public int UnitCount { get; protected set; }

        public Vector2i Target
        {
            get
            {
                return new Vector2i((int)TargetX/GameMap.TilePixelSize, (int)TargetY/GameMap.TilePixelSize);
            }
        }

        public void Join(Unit unit)
        {
            UnitCount++;
            if(unit.UnitGroup!=null)
            {
                unit.UnitGroup.Leave(unit);
            }
            unit.UnitGroup = this;
        }

        public void Leave(Unit unit)
        {
            UnitCount--;
            unit.UnitGroup = null;
        }

        public virtual void Update()
        {

        }

        public virtual void UpdateUnitComplete(Unit u)
        {
            if (u.UnitGroup.Target.Equals(GameMap.ToTilePos(u.Position)))
            {
                u.UnitGroup.Leave(u);
            }
        }

        public virtual bool HasGroupBeenCompleted()
        {
            return UnitCount <= 0;
        }
    }
}
