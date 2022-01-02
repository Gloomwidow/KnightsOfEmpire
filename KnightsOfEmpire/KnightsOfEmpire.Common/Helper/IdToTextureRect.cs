using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Helper
{
    public static class IdToTextureRect
    {
        public static IntRect GetRect(int id, int sizeX, int sizeY, int size = 16)
        {
            int texCountX = sizeX / size;
            int posY = id / texCountX;
            int posX = id % texCountX;
            return new IntRect(posX * size, posY * size, size, size);
        }

        public static IntRect GetRect(int id, Vector2i texSize, int size = 16)
        {
            return GetRect(id, texSize.X, texSize.Y, size);
        }
    }
}
