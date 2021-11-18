using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Units
{
    /// <summary>
    /// A response from server informing client to register new unit on their side
    /// </summary>
    [Serializable]
    public class RegisterUnitRequest
    {
        /// <summary>
        /// To which player belongs that unit
        /// </summary>
        public int PlayerId { get; set; }
        /// <summary>
        /// Which unit is it from player roster
        /// </summary>
        public int UnitTypeId { get; set; }
        /// <summary>
        /// Unique id to identify unit during updates
        /// </summary>
        public char[] ID { get; set; }
        public float StartPositionX { get; set; }
        public float StartPositionY { get; set; }
    }
}
