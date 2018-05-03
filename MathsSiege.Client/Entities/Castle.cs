using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using System.Diagnostics;

namespace MathsSiege.Client.Entities
{
    public class Castle : AttackableEntity, IEnemyTarget
    {
        private const int HEAL_INTERVAL = 5000;
        private const int HEAL_AMOUNT = 5;

        public Vector2 Position { get; set; }

        private Sprite sprite;

        private Stopwatch stopwatch = new Stopwatch();

        public Castle(Texture2D texture)
        {
            sprite = new Sprite(texture);
        }

        public bool ContainsTile(Tile tile)
        {
            return new RectangleF(Position.X, Position.Y, 3, 3).Contains(tile.Position);
        }

        public override void OnAddedToScene()
        {
            var map = Scene.Services.GetService<GameMap>();
            sprite.Position = map.MapToScreen(Position);
            sprite.Depth = (Position.Y / map.TiledMap.Height) * (Position.X / map.TiledMap.Width);
            stopwatch.Start();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (stopwatch.ElapsedMilliseconds > HEAL_INTERVAL)
            {
                Health += HEAL_AMOUNT;
                stopwatch.Restart();
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
    }
}
