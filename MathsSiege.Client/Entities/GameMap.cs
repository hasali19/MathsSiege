using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;

namespace MathsSiege.Client.Entities
{
    public class GameMap : DrawableEntity
    {
        private TiledMap map;
        private TiledMapRenderer renderer;

        public GameMap(TiledMap map)
        {
            this.map = map;
        }

        public override void OnAddedToScene()
        {
            this.renderer = new TiledMapRenderer(this.Scene.GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            this.renderer.Update(this.map, gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var viewMatrix = this.Scene.Camera.GetViewMatrix();
            this.renderer.Draw(this.map, viewMatrix);
        }
    }
}
