using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace MathsSiege.Client.Entities
{
    public class Defence : AttackableEntity
    {
        public Vector2 Position { get; set; }

        private Sprite sprite;

        public Defence(TextureAtlas atlas)
        {
            this.sprite = new Sprite(atlas.GetRegion("Down"))
            {
                Origin = new Vector2(32, 32)
            };
        }

        public override void OnAddedToScene()
        {
            var map = this.Scene.Services.GetService<GameMap>();
            this.sprite.Position = map.MapToScreen(this.Position);
            this.sprite.Depth = (this.Position.Y / map.TiledMap.Height) * (this.Position.X / map.TiledMap.Width);
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
