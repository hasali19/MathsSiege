using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Collections;
using MonoGame.Extended.TextureAtlases;
using System.Collections.Generic;

namespace MathsSiege.Client.Entities
{
    public class TrapManager : DrawableEntity
    {
        private IDictionary<Tile, Trap> traps = new Dictionary<Tile, Trap>();
        private Bag<Tile> trapsToRemove = new Bag<Tile>();

        private TextureAtlas spikesTextureAtlas;

        /// <summary>
        /// Creates a new trap on the specified tile.
        /// </summary>
        /// <param name="type">The type of trap to create.</param>
        /// <param name="tile">The tile on which to place the trap.</param>
        /// <returns></returns>
        public bool CreateTrap(string type, Tile tile)
        {
            if (!tile.IsPlaceable)
            {
                return false;
            }

            Trap trap;

            switch (type)
            {
                case DefenceTypes.Spikes:
                    trap = new SpikesTrap(spikesTextureAtlas);
                    break;

                default:
                    return false;
            }

            trap.Scene = Scene;
            trap.Position = tile.Position;

            traps.Add(tile, trap);
            trap.OnAddedToScene();

            trap.Destroyed += (t) => trapsToRemove.Add(tile);

            return true;
        }

        /// <summary>
        /// Removes a trap from the specified tile.
        /// </summary>
        /// <param name="tile">The tile to remove a trap from.</param>
        /// <returns>True if a trap was successfully removed.</returns>
        public bool RemoveTrap(Tile tile)
        {
            if (!CheckContainsTrap(tile))
            {
                return false;
            }

            var defence = traps[tile];
            defence.Scene = null;
            defence.OnRemovedFromScene();
            traps.Remove(tile);

            return true;
        }

        /// <summary>
        /// Checks if a tile contains a trap.
        /// </summary>
        /// <param name="tile">The tile to check.</param>
        /// <returns></returns>
        public bool CheckContainsTrap(Tile tile)
        {
            return traps.ContainsKey(tile);
        }

        public override void OnAddedToScene()
        {
            spikesTextureAtlas = Scene.Content.Load<TextureAtlas>(ContentPaths.Textures.SpikesAtlas);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var trap in traps.Values)
            {
                trap.Update(gameTime);
            }

            foreach (var tile in trapsToRemove)
            {
                RemoveTrap(tile);
            }

            trapsToRemove.Clear();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var trap in traps.Values)
            {
                trap.Draw(gameTime);
            }
        }
    }
}
