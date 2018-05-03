using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;

namespace MathsSiege.Client.Entities
{
    public class Wall : AttackableEntity, IEnemyTarget
    {
        public Vector2 Position { get; set; }
        
        private Sprite sprite;

        public Wall(Texture2D texture)
        {
            sprite = new Sprite(texture)
            {
                Origin = new Vector2(32, 32)
            };
        }

        public override void OnAddedToScene()
        {
            var map = Scene.Services.GetService<GameMap>();
            sprite.Position = map.MapToScreen(Position);
            sprite.Depth = (Position.Y / map.TiledMap.Height) * (Position.X / map.TiledMap.Width);
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
