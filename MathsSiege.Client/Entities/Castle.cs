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
            this.sprite = new Sprite(texture);
        }

        public bool ContainsTile(Tile tile)
        {
            return new RectangleF(this.Position.X, this.Position.Y, 3, 3).Contains(tile.Position);
        }

        public override void OnAddedToScene()
        {
            var map = this.Scene.Services.GetService<GameMap>();
            this.sprite.Position = map.MapToScreen(this.Position);
            this.sprite.Depth = (this.Position.Y / map.TiledMap.Height) * (this.Position.X / map.TiledMap.Width);
            this.stopwatch.Start();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.stopwatch.ElapsedMilliseconds > HEAL_INTERVAL)
            {
                this.Health += HEAL_AMOUNT;
                this.stopwatch.Restart();
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
    }
}
