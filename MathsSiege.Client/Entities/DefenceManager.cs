using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
using System.Collections.Generic;

namespace MathsSiege.Client.Entities
{
    public class DefenceManager : DrawableEntity
    {
        private IDictionary<Tile, Defence> defences = new Dictionary<Tile, Defence>();

        private TextureAtlas cannonTextureAtlas;

        /// <summary>
        /// Creates a new defence at the specified tile.
        /// </summary>
        /// <param name="type">The type of defence to create.</param>
        /// <param name="tile">The tile to place the defence on.</param>
        /// <returns>True if the defence was created successfully.</returns>
        public bool CreateDefence(string type, Tile tile)
        {
            if (!tile.IsPlaceable || this.CheckContainsDefence(tile))
            {
                return false;
            }

            Defence defence;

            switch (type)
            {
                case DefenceTypes.Cannon:
                    defence = new Defence(this.cannonTextureAtlas);
                    break;

                default:
                    return false;
            }

            defence.Scene = this.Scene;
            defence.Position = tile.Position;

            this.defences.Add(tile, defence);
            defence.OnAddedToScene();

            return true;
        }

        /// <summary>
        /// Removes a defence from the specified tile.
        /// </summary>
        /// <param name="tile">The tile to remove a defence from.</param>
        /// <returns>True if a defence was successfully removed.</returns>
        public bool RemoveDefence(Tile tile)
        {
            if (!this.CheckContainsDefence(tile))
            {
                return false;
            }

            this.defences[tile].Scene = null;
            this.defences[tile].OnRemovedFromScene();
            this.defences.Remove(tile);

            return true;
        }

        /// <summary>
        /// Checks if a tile contains a defence.
        /// </summary>
        /// <param name="tile">The tile to check.</param>
        /// <returns></returns>
        public bool CheckContainsDefence(Tile tile)
        {
            return this.defences.ContainsKey(tile);
        }

        public override void OnAddedToScene()
        {
            this.cannonTextureAtlas = this.Scene.Content.Load<TextureAtlas>(ContentPaths.Textures.CannonAtlas);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var defence in this.defences.Values)
            {
                defence.Draw(gameTime);
            }
        }
    }
}
