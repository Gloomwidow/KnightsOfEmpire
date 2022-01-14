using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
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

        public const int MaxUnitsPerPlayer = 32;

        public const int MaxUpgradesPerUnit = 4;

        public static string MapsDirectory
        {
            get
            {
                return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @".\Assets\Maps"));
            }
        }

        public static string ConfigFile
        {
            get
            {
                return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @".\server.config"));
            }
        }
    }
}
