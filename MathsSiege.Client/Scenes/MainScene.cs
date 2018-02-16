using MathsSiege.Client.Entities;
using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Tiled;

namespace MathsSiege.Client.Scenes
{
    /// <summary>
    /// The main game scene.
    /// </summary>
    public class MainScene : Scene
    {
        public MainScene(Game game) : base(game)
        {
            this.ClearColor = Color.White;

            this.Game.IsMouseVisible = true;
        }

        public override void Initialise()
        {
            base.Initialise();

            #region Load content
            var background = this.Content.Load<Texture2D>(ContentPaths.Textures.Background);
            var map = this.Content.Load<TiledMap>(ContentPaths.TiledMap.Map);
            #endregion

            this.BackgroundImage = background;

            // Initialise the game map.
            this.AddEntity(new GameMap(map));

            // Center the camera.
            this.Camera.LookAt(Vector2.Zero);
        }
    }
}
