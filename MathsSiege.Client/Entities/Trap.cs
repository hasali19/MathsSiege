using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MathsSiege.Client.Entities
{
    public enum TrapState
    {
        Idle,
        Triggered
    }

    public abstract class Trap : DrawableEntity
    {
        public Vector2 Position { get; set; }

        public event Action<Trap> Destroyed;

        private TextureAtlas atlas;
        private Sprite sprite;

        private EnemyManager enemyManager;

        private Stopwatch stopwatch = new Stopwatch();

        public Trap(TextureAtlas atlas)
        {
            this.atlas = atlas;
            this.sprite = new Sprite(this.GetTextureAtlasRegion(TrapState.Idle))
            {
                Origin = new Vector2(32, 0)
            };
        }

        public override void OnAddedToScene()
        {
            var map = this.Scene.Services.GetService<GameMap>();
            this.enemyManager = this.Scene.Services.GetService<EnemyManager>();

            this.sprite.Position = map.MapToScreen(this.Position);
            this.sprite.Depth = (this.Position.Y / map.TiledMap.Height) * (this.Position.X / map.TiledMap.Width);

            this.stopwatch.Start();
        }

        public override void Update(GameTime gameTime)
        {
            // Check for enemies in range every half a second.
            if (this.stopwatch.ElapsedMilliseconds > 500)
            {
                var enemies = this.enemyManager.GetEnemiesInRange(this.Position, 1f);

                // Trigger if there are enemies in range.
                if (enemies.Count > 0)
                {
                    this.stopwatch.Reset();
                    this.OnTrigger(enemies);
                }
                else
                {
                    this.stopwatch.Restart();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            this.Scene.SpriteBatch.Draw(this.sprite);
        }

        /// <summary>
        /// Sets the sprite's texture region for a particular state.
        /// </summary>
        /// <param name="state"></param>
        protected void SetTextureRegion(TrapState state)
        {
            this.sprite.TextureRegion = this.GetTextureAtlasRegion(state);
        }

        /// <summary>
        /// Performs the trap's triggered action, to be implemented
        /// by individual trap types.
        /// </summary>
        /// <param name="enemies"></param>
        protected abstract void OnTrigger(IReadOnlyCollection<Enemy> enemies);

        /// <summary>
        /// Invokes the trap's <see cref="Destroyed"/> event.
        /// </summary>
        protected virtual void OnDestroyed()
        {
            this.Destroyed?.Invoke(this);
        }

        /// <summary>
        /// Gets the texture atlas region for a particular state.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private TextureRegion2D GetTextureAtlasRegion(TrapState state)
        {
            return this.atlas.GetRegion(state.ToString());
        }
    }
}
