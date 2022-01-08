using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.Common.Extensions;

namespace KnightsOfEmpire.Common.Helper
{
    public static class Intersections
    {
        // Based on: https://www.geeksforgeeks.org/check-if-two-given-line-segments-intersect/
        private static bool BetweenExtremes(float a, float b, float c)
        {
            return c <= Math.Max(a, b) && c >= Math.Min(a, b);
        }

        private static bool IsOnLineSegment(Vector2f s, Vector2f e, Vector2f p)
        {
            return BetweenExtremes(s.X, e.X, p.X) && BetweenExtremes(s.Y, e.Y, p.Y);
        }

        private static int GetOrientation(Vector2f p, Vector2f q, Vector2f r)
        {
            float val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);
            if (val == 0) return 0; //collinear
            return val > 0 ? 1 : 2; // clockwise / counter - clockwise
        }

        public static bool LineSegmentsCollide(Vector2f s1, Vector2f e1, Vector2f s2, Vector2f e2)
        {
            int o1 = GetOrientation(s1, e1, s2);
            int o2 = GetOrientation(s1, e1, e2);
            int o3 = GetOrientation(s2, e2, s1);
            int o4 = GetOrientation(s2, e2, e1);

            if (o1 != o2 && o3 != o4) return true;
            if (o1 == 0 && IsOnLineSegment(s1, e1, s2)) return true;
            if (o2 == 0 && IsOnLineSegment(s1, e1, e2)) return true;
            if (o3 == 0 && IsOnLineSegment(s2, e2, s1)) return true;
            if (o4 == 0 && IsOnLineSegment(s2, e2, e1)) return true;
            return false;
        }

        public static bool LineSegmentCollideWithTile(Vector2f start, 
            Vector2f end, Vector2i tilePos)
        {
            float s = Map.Map.TilePixelSize;
            if(LineSegmentsCollide(start, end,
                new Vector2f((tilePos.X)*s,(tilePos.Y)*s),
                new Vector2f((tilePos.X+1) * s, (tilePos.Y) * s))) return true;
            if (LineSegmentsCollide(start, end,
                new Vector2f((tilePos.X) * s, (tilePos.Y) * s),
                new Vector2f((tilePos.X) * s, (tilePos.Y+1) * s))) return true;
            if (LineSegmentsCollide(start, end,
                new Vector2f((tilePos.X+1) * s, (tilePos.Y) * s),
                new Vector2f((tilePos.X+1) * s, (tilePos.Y+1) * s))) return true;
            if (LineSegmentsCollide(start, end,
                new Vector2f((tilePos.X) * s, (tilePos.Y+1) * s),
                new Vector2f((tilePos.X + 1) * s, (tilePos.Y + 1) * s))) return true;

            return false;
        }

        public static bool CollidesWithMap(Vector2f start, Vector2f end, List<Vector2i> wallTiles, float distance2)
        {
            foreach(Vector2i tile in wallTiles)
            {
                Vector2f tileMiddle = new Vector2f((tile.X + 0.5f) * Map.Map.TilePixelSize, (tile.Y + 0.5f) * Map.Map.TilePixelSize);
                if(tileMiddle.Distance(start)<=distance2)
                {
                    if (LineSegmentCollideWithTile(start, end, tile)) return true;
                }
            }
            return false;

        }
    }
}
