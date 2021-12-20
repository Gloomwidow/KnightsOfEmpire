using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Buildings
{
    [Serializable]
    public class RegisterBuildingRequest
    {
        public int PlayerId { get; set; }
        public int BuildingTypeId { get; set; }
        public int DestroyPosX { get; set; }
        public int DestroyPosY { get; set; }
    }
}
