using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;

namespace MathsSiege.Client.Entities
{
    public class HoveredTileOverlay : DrawableEntity
    {
        private GameMap map;
        private Sprite sprite;

        public HoveredTileOverlay(Texture2D texture)
        {
            this.sprite = new Sprite(texture)
            {
                Origin = new Vector2(texture.Width / 2, 0)
            };
        }

        public override void OnAddedToScene()
        {
            this.map = this.Scene.Services.GetService<GameMap>();
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.map.HoveredTile != null)
            {
                var position = this.map.MapToScreen(this.map.HoveredTile.Position);

                var color = this.map.HoveredTile.IsPlaceable
                    ? Color.White * 0.5f
                    : Color.Red * 0.5f;

                this.sprite.Position = position;
                this.sprite.Color = color;

                this.Scene.SpriteBatch.Draw(this.sprite);
            }
        }
    }
}
