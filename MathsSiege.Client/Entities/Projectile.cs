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
            sprite = new Sprite(texture)
            {
                Origin = new Vector2(texture.Width / 2, texture.Height / 2),
                Depth = 1
            };
        }

        public void Fire(Vector2 initial, Vector2 target, float angle = 30)
        {
            initialPosition = new Vector3(initial, 0);
            targetPosition = new Vector3(target, 0);
            position = initialPosition;

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
            initialVelocity = new Vector3(
                (target - initial).NormalizedCopy() * speed * (float)Math.Cos(radians),
                speed * (float)Math.Sin(radians));

            stopwatch.Start();
        }

        public override void OnAddedToScene()
        {
            map = Scene.Services.GetService<GameMap>();
            enemyManager = Scene.Services.GetService<EnemyManager>();
        }

        public override void Update(GameTime gameTime)
        {
            // Check if the projectile has reached the ground.
            if (position.Z < 0)
            {
                var enemies = enemyManager.GetEnemiesInRange(new Vector2(position.X, position.Y), 1.5f);

                foreach (var enemy in enemies)
                {
                    enemy.Attack(20);
                }

                stopwatch.Stop();
                TargetReached?.Invoke(this);
            }

            // Calculate the current position from the elapsed time.
            var time = (float)stopwatch.Elapsed.TotalSeconds;
            displacement = (initialVelocity * time) + (0.5f * (Acceleration * (float)Math.Pow(time, 2)));
            position = initialPosition + displacement;

            // Update the actual position on the screen.
            sprite.Position = map.MapToScreen(new Vector2(position.X, position.Y))
                + new Vector2(0, -position.Z * (map.TiledMap.TileHeight / 2));
        }

        public override void Draw(GameTime gameTime)
        {
            Scene.SpriteBatch.Draw(sprite);
        }
    }
}
