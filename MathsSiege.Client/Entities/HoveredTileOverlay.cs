using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MathsSiege.Client.Entities
{
    public class HoveredTileOverlay : DrawableEntity
    {
        private GameMap map;
        private Texture2D texture;

        public Tile CurrentTile { get; set; }

        public HoveredTileOverlay(GameMap map, Texture2D texture)
        {
            this.map = map;
            this.texture = texture;
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var worldPosition = this.Scene.Camera.ScreenToWorld(mouseState.Position.ToVector2());
            var tilePosition = this.map.TiledMap.ScreenToMap(worldPosition);
            this.CurrentTile = this.map[(int)tilePosition.X, (int)tilePosition.Y];
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.CurrentTile != null)
            {
                var position = this.map.TiledMap.MapToScreen(this.CurrentTile.Position);

                var color = this.CurrentTile.IsPlaceable
                    ? Color.White * 0.5f
                    : Color.Red * 0.5f;

                this.Scene.SpriteBatch.Draw(this.texture, position, color);
            }
        }
    }
}
