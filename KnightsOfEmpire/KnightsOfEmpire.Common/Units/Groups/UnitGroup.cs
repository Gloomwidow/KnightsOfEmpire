using KnightsOfEmpire.Common.Extensions;
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
        protected static int NextGroupID = 0;
        public const int FinishDistance = 16;

        public int GroupID { get; protected set; }
        public float TargetX;
        public float TargetY;

        public bool IgnoresAttackRange = false;

        public int UnitCount { get; protected set; }

        public Vector2f PreciseTarget
        {
            get
            {
                return new Vector2f(TargetX, TargetY);
            }
        }

        public Vector2i Target
        {
            get
            {
                return Map.Map.ToTilePos(PreciseTarget);
            }
        }

        public UnitGroup()
        {
            GroupID = NextGroupID++;
        }

        public override bool Equals(object obj)
        {
            if (obj is UnitGroup) return ((UnitGroup)(obj)).GroupID == this.GroupID;
            return false;
        }

        public void Join(Unit unit)
        {
            UnitCount++;
            if (unit.UnitGroup!=null)
            {
                unit.UnitGroup.Leave(unit);
            }
            unit.IsGroupCompleted = false;
            unit.UnitGroup = this;
        }

        public void CompleteGroup(Unit unit)
        {
            Leave(unit);
            unit.PreviousCompletedGroup = this;
        }

        public void Leave(Unit unit)
        {
            UnitCount--;
            unit.UnitGroup = null;
            unit.PreviousCompletedGroup = null;
        }

        public virtual void Update()
        {

        }

        public virtual void UpdateUnitComplete(Unit u)
        {
            //if (u.UnitGroup.Target.Equals(GameMap.ToTilePos(u.Position)))
            //{
            //    u.UnitGroup.CompleteGroup(u);
            //}

            if(u.Position.Distance2(new Vector2f(TargetX, TargetY))<=FinishDistance*FinishDistance)
            {
                u.UnitGroup.CompleteGroup(u);
            }
        }

        public virtual bool HasGroupBeenCompleted()
        {
            return UnitCount <= 0;
        }
    }
}
