using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Helper
{
    public static class Constants
    {
        public const int MaxPlayers = 4;

        public static readonly Color[] playerColors = new Color[]
            {
                Color.Green,
                Color.Yellow,
                Color.Blue,
                Color.Red
            };
    }
}
