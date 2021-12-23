using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Extensions
{
    public static class Vector2fExtension
    { 
        /// <summary>
        /// Calculates length of the vector.
        /// </summary>
        /// <param name="v">Vector</param>
        /// <returns>Lenght of the vector counted from (0,0) point</returns>
        public static float Length(this Vector2f v)
        {
            return (float)Math.Sqrt((v.X * v.X) + (v.Y * v.Y));
        }

        /// <summary>
        /// Calculates square length of the vector.
        /// </summary>
        /// <param name="v">Vector</param>
        /// <returns>Lenght of the vector counted from (0,0) point</returns>
        public static float Length2(this Vector2f v)
        {
            return (v.X * v.X) + (v.Y * v.Y);
        }
        /// <summary>
        /// Normalizes vector, so its length is equal to 1. Useful for phisics calculation, when you want to move object in direction with defined speed.
        /// </summary>
        /// <param name="v">Vector for normalization</param>
        public static void Normalize(this Vector2f v)
        {
            float l = v.Length();
            if (l != 0)
            {
                v.X /= l;
                v.Y /= l;
            }

        }
        /// <summary>
        /// Calculates normalized version of argument vector, which length is 1.
        /// </summary>
        /// <param name="v">Vector to normalize</param>
        /// <returns>A copy of normalized vector v. Vector v is unchanged.</returns>
        public static Vector2f Normalized(this Vector2f v)
        {
            Vector2f result = new Vector2f(v.X, v.Y);
            float l = v.Length();
            if (l != 0)
            {
                result.X /= l;
                result.Y /= l;
            }
            return result;
        }
        /// <summary>
        /// Calculates the square distance between two points
        /// </summary>
        /// <param name="a">First point</param>
        /// <param name="b">Second point</param>
        /// <returns>Square distance between two provided points</returns>
        public static float Distance2(this Vector2f a, Vector2f b)
        {
            return (float)((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
        }

        /// <summary>
        /// Calculates the distance between two points.
        /// !!!Attention!!! When calculating distance between two points, its recommended to use Distance2 instead of Distance if not needed,
        /// since calculating square root is time consuming operation. 
        /// For example, if you need to check if distance between two points (a,b) is smaller than 5 units, just do Distance2(a,b)<=5*5. 
        /// </summary>
        /// <param name="a">First point</param>
        /// <param name="b">Second point</param>
        /// <returns>Distance between two provided points</returns>
        public static float Distance(this Vector2f a, Vector2f b)
        {
            return (float)Math.Sqrt(a.Distance2(b));
        }

        public static Vector2f LerpTimeStep(this Vector2f start, Vector2f end, float rotationSpeed, float timeStep)
        {
            Vector2f delta = end - start;
            float rotation = rotationSpeed * timeStep;
            rotation = Math.Max(Math.Min(rotation, 1.0f), 0.0f);
            return start + (delta * rotation);
        }


        public static float Distance2Rect(this Vector2f p, FloatRect rect)
        {
            float dx = Math.Min(Math.Abs(rect.Left - p.X), Math.Abs(p.X - (rect.Left+rect.Width)));
            float dy = Math.Min(Math.Abs(rect.Top - p.Y), Math.Abs(p.Y - (rect.Top+rect.Height)));
            return dx * dx + dy * dy;
        }
    }
}
