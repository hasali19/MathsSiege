﻿using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;

namespace MathsSiege.Client.Entities
{
    public class DefenceManager : DrawableEntity
    {
        public int DefenceCount => this.defences.Count;

        public event Action<Defence> DefenceAdded;
        public event Action<Defence> DefenceRemoved;

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
            if (!tile.IsPlaceable)
            {
                return false;
            }

            Defence defence;

            switch (type)
            {
                case DefenceTypes.Cannon:
                    defence = new Cannon(this.cannonTextureAtlas);
                    break;

                default:
                    return false;
            }

            defence.Scene = this.Scene;
            defence.Position = tile.Position;

            this.defences.Add(tile, defence);
            defence.OnAddedToScene();

            defence.Destroyed += (attackable) => this.RemoveDefence(tile);

            this.DefenceAdded?.Invoke(defence);

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

            var defence = this.defences[tile];
            defence.Scene = null;
            defence.OnRemovedFromScene();
            this.defences.Remove(tile);

            this.DefenceRemoved?.Invoke(defence);

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

        /// <summary>
        /// Gets the nearest defence to a particular position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Defence GetNearestDefence(Vector2 position)
        {
            Defence nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (var defence in this.defences.Values)
            {
                // Use the square of length to avoid square roots.
                float distance = (defence.Position - position).LengthSquared();
                if (nearest == null || distance < nearestDistance)
                {
                    nearest = defence;
                    nearestDistance = distance;
                }
            }

            return nearest;
        }

        public override void OnAddedToScene()
        {
            this.cannonTextureAtlas = this.Scene.Content.Load<TextureAtlas>(ContentPaths.Textures.CannonAtlas);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var defence in this.defences.Values)
            {
                defence.Update(gameTime);
            }
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
