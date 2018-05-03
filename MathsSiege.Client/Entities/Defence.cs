using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System.Diagnostics;

namespace MathsSiege.Client.Entities
{
    public abstract class Defence : AttackableEntity, IEnemyTarget
    {
        public Vector2 Position { get; set; }

        public int AttackInterval { get; set; } = 1500;

        public float AttackRange { get; set; } = 10f;

        private EnemyManager enemyManager;

        private TextureAtlas atlas;
        private Sprite sprite;

        private Direction Facing;
        private Enemy target;

        private Stopwatch stopwatch = new Stopwatch();

        public Defence(TextureAtlas atlas)
        {
            this.atlas = atlas;
            sprite = new Sprite(GetAtlasRegion(Facing))
            {
                Origin = new Vector2(32, 32)
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
            // Search for the nearest enemy in range. Do this every tick
            // so that the target is always the closest possible one.
            target = enemyManager.GetNearestEnemyInRange(Position, AttackRange);

            if (target != null)
            {
                var displacement = target.Position - Position;
                var direction = Utilities.GetDirectionFromVector(displacement);

                if (Facing != direction)
                {
                    Facing = direction;
                    sprite.TextureRegion = GetAtlasRegion(Facing);
                }

                // Perform an attack if enough time has passed.
                if (stopwatch.ElapsedMilliseconds > AttackInterval)
                {
                    DoAttack(target);
                    stopwatch.Restart();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Scene.SpriteBatch.Draw(sprite);

            if (Health < MaxHealth)
            {
                var healthbar = new RectangleF(sprite.Position.X - 40, sprite.Position.Y - 32, 80, 10);
                DrawHealthbar(healthbar, Color.Red);
            }
        }

        /// <summary>
        /// Performs an attack action. The precise behaviour is
        /// specific to each defence type.
        /// </summary>
        /// <param name="target"></param>
        protected abstract void DoAttack(Enemy target);

        /// <summary>
        /// Gets the texture region for the given direction.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        private TextureRegion2D GetAtlasRegion(Direction direction)
        {
            return atlas.GetRegion(direction.ToString());
        }
    }
}
