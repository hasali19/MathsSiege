using Microsoft.Xna.Framework;
using System;

namespace MathsSiege.Client
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    public static class Utilities
    {
        /// <summary> 
        /// Gets the direction (up, down, left, right) of a vector. 
        /// </summary> 
        /// <param name="vector"></param> 
        /// <returns>A direction.</returns> 
        public static Direction GetDirectionFromVector(Vector2 vector)
        {
            if (Math.Abs(vector.X) > Math.Abs(vector.Y))
            {
                if (vector.X <= 0)
                {
                    return Direction.Left;
                }
                else
                {
                    return Direction.Right;
                }
            }
            else
            {
                if (vector.Y >= 0)
                {
                    return Direction.Down;
                }
                else
                {
                    return Direction.Up;
                }
            }
        }
    }
}
