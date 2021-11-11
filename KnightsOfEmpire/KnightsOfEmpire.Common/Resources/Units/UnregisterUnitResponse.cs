using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Units
{
    /// <summary>
    /// A response from server informing client to unregister (delete) unit in case of its death for instance
    /// </summary>
    [Serializable]
    public class UnregisterUnitResponse
    {
        /// <summary>
        /// To which player belongs that unit
        /// </summary>
        public int PlayerId { get; set; }
        /// <summary>
        /// Unique id to identify which unit is deleted
        /// </summary>
        public char[] ID { get; set; }
    }
}
