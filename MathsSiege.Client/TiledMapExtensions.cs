using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using System;

namespace MathsSiege.Client
{
    public static class TiledMapExtensions
    {
        /// <summary>
        /// Gets the tile position of the specfied screen coordinates.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public static Vector2 ScreenToMap(this TiledMap map, Vector2 coordinates)
        {
            switch (map.Orientation)
            {
                case TiledMapOrientation.Orthogonal:
                    return coordinates;

                case TiledMapOrientation.Isometric:
                    return IsometricScreenToMap(map, coordinates);

                default:
                    throw new ArgumentException("Unsupported map orientation: " + map.Orientation.ToString());
            }
        }

        /// <summary>
        /// Gets the screen position of the specified tile coordinates.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public static Vector2 MapToScreen(this TiledMap map, Vector2 coordinates)
        {
            switch (map.Orientation)
            {
                case TiledMapOrientation.Orthogonal:
                    return coordinates;

                case TiledMapOrientation.Isometric:
                    return IsometricMapToScreen(map, coordinates);

                default:
                    throw new ArgumentException("Unsupported map orientation: " + map.Orientation.ToString());
            }
        }

        private static Vector2 IsometricScreenToMap(TiledMap map, Vector2 coordinates)
        {
            var tileWidthHalf = map.TileWidth / 2f;
            var tileHeightHalf = map.TileHeight / 2f;

            return new Vector2
            {
                X = (coordinates.X / tileWidthHalf + coordinates.Y / tileHeightHalf) / 2,
                Y = (coordinates.Y / tileHeightHalf - (coordinates.X / tileWidthHalf)) / 2
            };
        }

        private static Vector2 IsometricMapToScreen(TiledMap map, Vector2 coordinates)
        {
            var tileWidthHalf = map.TileWidth / 2f;
            var tileHeightHalf = map.TileHeight / 2f;

            return new Vector2
            {
                X = (coordinates.X - coordinates.Y) * tileWidthHalf - tileWidthHalf,
                Y = (coordinates.X + coordinates.Y) * tileHeightHalf
            };
        }
    }
}
