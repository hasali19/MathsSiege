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
            sprite = new Sprite(texture)
            {
                Origin = new Vector2(texture.Width / 2, 0)
            };
        }

        public override void OnAddedToScene()
        {
            map = Scene.Services.GetService<GameMap>();
        }

        public override void Draw(GameTime gameTime)
        {
            if (map.HoveredTile != null)
            {
                var position = map.MapToScreen(map.HoveredTile.Position);

                var color = map.HoveredTile.IsPlaceable
                    ? Color.White * 0.5f
                    : Color.Red * 0.5f;

                sprite.Position = position;
                sprite.Color = color;

                Scene.SpriteBatch.Draw(sprite);
            }
        }
    }
}
