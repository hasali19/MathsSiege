using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using System;
using System.Diagnostics;

namespace MathsSiege.Client.Entities
{
    public class Projectile : DrawableEntity
    {
        public Vector3 Acceleration { get; set; } = new Vector3(0, 0, -10f);

        public event Action<Projectile> TargetReached;

        private GameMap map;
        private EnemyManager enemyManager;

        private Sprite sprite;

        private Vector3 initialVelocity;
        private Vector3 initialPosition;
        private Vector3 targetPosition;
        private Vector3 displacement;
        private Vector3 position;

        private Stopwatch stopwatch = new Stopwatch();

        public Projectile(Texture2D texture)
        {
            this.sprite = new Sprite(texture)
            {
                Origin = new Vector2(texture.Width / 2, texture.Height / 2),
                Depth = 1
            };
        }

        public void Fire(Vector2 initial, Vector2 target, float angle = 30)
        {
            this.initialPosition = new Vector3(initial, 0);
            this.targetPosition = new Vector3(target, 0);
            this.position = this.initialPosition;

            // Calculate the distance between the initial
            // position and the target.
            float distance = (target - initial).Length();
            // Convert the angle into radians.
            float radians = MathHelper.ToRadians(angle);
            // Calculate the neccessary speed to travel the
            // required distance at the given angle.
            float speed = (float)Math.Sqrt((distance * Acceleration.Z) / (-Math.Sin(2 * radians)));

            // Calculate the initial velocity according to
            // the calculated speed and given angle.
            this.initialVelocity = new Vector3(
                (target - initial).NormalizedCopy() * speed * (float)Math.Cos(radians),
                speed * (float)Math.Sin(radians));

            this.stopwatch.Start();
        }

        public override void OnAddedToScene()
        {
            this.map = this.Scene.Services.GetService<GameMap>();
            this.enemyManager = this.Scene.Services.GetService<EnemyManager>();
        }

        public override void Update(GameTime gameTime)
        {
            // Check if the projectile has reached the ground.
            if (this.position.Z < 0)
            {
                var enemies = this.enemyManager.GetEnemiesInRange(new Vector2(this.position.X, this.position.Y), 1.5f);

                foreach (var enemy in enemies)
                {
                    enemy.Attack(20);
                }

                this.stopwatch.Stop();
                this.TargetReached?.Invoke(this);
            }

            // Calculate the current position from the elapsed time.
            var time = (float)this.stopwatch.Elapsed.TotalSeconds;
            this.displacement = (this.initialVelocity * time) + (0.5f * (Acceleration * (float)Math.Pow(time, 2)));
            this.position = this.initialPosition + this.displacement;

            // Update the actual position on the screen.
            this.sprite.Position = this.map.MapToScreen(new Vector2(this.position.X, this.position.Y))
                + new Vector2(0, -this.position.Z * (this.map.TiledMap.TileHeight / 2));
        }

        public override void Draw(GameTime gameTime)
        {
            this.Scene.SpriteBatch.Draw(this.sprite);
        }
    }
}
