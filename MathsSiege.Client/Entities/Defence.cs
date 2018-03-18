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
            this.sprite = new Sprite(this.GetAtlasRegion(this.Facing))
            {
                Origin = new Vector2(32, 32)
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
            // Search for the nearest enemy in range. Do this every tick
            // so that the target is always the closest possible one.
            this.target = this.enemyManager.GetNearestEnemyInRange(this.Position, this.AttackRange);

            if (this.target != null)
            {
                var displacement = this.target.Position - this.Position;
                var direction = Utilities.GetDirectionFromVector(displacement);

                if (this.Facing != direction)
                {
                    this.Facing = direction;
                    this.sprite.TextureRegion = this.GetAtlasRegion(this.Facing);
                }

                // Perform an attack if enough time has passed.
                if (this.stopwatch.ElapsedMilliseconds > this.AttackInterval)
                {
                    this.DoAttack(this.target);
                    this.stopwatch.Restart();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            this.Scene.SpriteBatch.Draw(this.sprite);

            if (this.Health < this.MaxHealth)
            {
                var healthbar = new RectangleF(this.sprite.Position.X - 40, this.sprite.Position.Y - 32, 80, 10);
                this.DrawHealthbar(healthbar, Color.Red);
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
            return this.atlas.GetRegion(direction.ToString());
        }
    }
}
