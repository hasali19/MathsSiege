using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        public bool IsTriggeredByFlying { get; set; }

        public event Action<Trap> Destroyed;

        private TextureAtlas atlas;
        private Sprite sprite;

        private EnemyManager enemyManager;

        private Stopwatch stopwatch = new Stopwatch();

        public Trap(TextureAtlas atlas)
        {
            this.atlas = atlas;
            sprite = new Sprite(GetTextureAtlasRegion(TrapState.Idle))
            {
                Origin = new Vector2(32, 0)
            };
        }

        public override void OnAddedToScene()
        {
            var map = Scene.Services.GetService<GameMap>();
            enemyManager = Scene.Services.GetService<EnemyManager>();

            sprite.Position = map.MapToScreen(Position);
            sprite.Depth = (Position.Y / map.TiledMap.Height) * (Position.X / map.TiledMap.Width);

            stopwatch.Start();
        }

        public override void Update(GameTime gameTime)
        {
            // Check for enemies in range every half a second.
            if (stopwatch.ElapsedMilliseconds > 500)
            {
                var enemies = enemyManager.GetEnemiesInRange(Position, 1f);

                // Trigger if there are enemies in range.
                if (enemies.Count > 0 && (IsTriggeredByFlying || enemies.Any(e => !e.IsFlying)))
                {
                    stopwatch.Reset();
                    OnTrigger(enemies);
                }
                else
                {
                    stopwatch.Restart();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Scene.SpriteBatch.Draw(sprite);
        }

        /// <summary>
        /// Sets the sprite's texture region for a particular state.
        /// </summary>
        /// <param name="state"></param>
        protected void SetTextureRegion(TrapState state)
        {
            sprite.TextureRegion = GetTextureAtlasRegion(state);
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
            Destroyed?.Invoke(this);
        }

        /// <summary>
        /// Gets the texture atlas region for a particular state.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private TextureRegion2D GetTextureAtlasRegion(TrapState state)
        {
            return atlas.GetRegion(state.ToString());
        }
    }
}
