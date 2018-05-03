using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MathsSiege.Client.Entities
{
    public class WallManager : DrawableEntity
    {
        public event Action<Wall> WallAdded;
        public event Action<Wall> WallRemoved;

        private IDictionary<Tile, Wall> walls = new Dictionary<Tile, Wall>();

        private Texture2D texture;

        public WallManager(Texture2D texture)
        {
            this.texture = texture;
        }

        /// <summary>
        /// Creates a new wall at the specified tile.
        /// </summary>
        /// <param name="tile">The tile to create a wall at.</param>
        /// <returns>True if the wall was created successfully.</returns>
        public bool CreateWall(Tile tile)
        {
            if (!tile.IsPlaceable)
            {
                return false;
            }

            Wall wall = new Wall(texture)
            {
                Scene = Scene,
                Position = tile.Position,
            };

            walls.Add(tile, wall);
            wall.OnAddedToScene();

            wall.Destroyed += (attackable) => RemoveWall(tile);

            WallAdded?.Invoke(wall);

            return true;
        }

        /// <summary>
        /// Removes a wall from the specified tile.
        /// </summary>
        /// <param name="tile">The tile to remove a wall from.</param>
        /// <returns>True if a wall was successfully removed.</returns>
        public bool RemoveWall(Tile tile)
        {
            if (!CheckContainsWall(tile, out _))
            {
                return false;
            }

            var wall = walls[tile];
            wall.Scene = null;
            wall.OnRemovedFromScene();
            walls.Remove(tile);

            WallRemoved?.Invoke(wall);

            return true;
        }

        /// <summary>
        /// Checks if a tile contains a wall.
        /// </summary>
        /// <param name="tile">The tile to check.</param>
        /// <returns></returns>
        public bool CheckContainsWall(Tile tile, out Wall wall)
        {
            return walls.TryGetValue(tile, out wall);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var wall in walls.Values)
            {
                wall.Draw(gameTime);
            }
        }
    }
}
