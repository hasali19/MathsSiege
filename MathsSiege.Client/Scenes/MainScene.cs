using MathsSiege.Client.Entities;
using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Tiled;

namespace MathsSiege.Client.Scenes
{
    /// <summary>
    /// The main game scene.
    /// </summary>
    public class MainScene : Scene
    {
        private const int CameraMovementSpeed = 10;

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
            var tileOverlay = this.Content.Load<Texture2D>(ContentPaths.Textures.TileOverlay);
            var wall = this.Content.Load<Texture2D>(ContentPaths.Textures.Wall);
            #endregion

            this.BackgroundImage = background;
            
            var gameMap = new GameMap(map);
            var hoveredTileOverlay = new HoveredTileOverlay(tileOverlay);
            var wallManager = new WallManager(wall);

            this.Services.AddService(gameMap);
            this.Services.AddService(wallManager);

            this.AddEntity(gameMap);
            this.AddEntity(hoveredTileOverlay);
            this.AddEntity(wallManager);

            // Center the camera.
            this.Camera.LookAt(Vector2.Zero);

            var mouseListener = this.Game.Services.GetService<MouseListener>();

            mouseListener.MouseClicked += (sender, args) =>
            {
                if (args.Button == MouseButton.Left && gameMap.HoveredTile != null)
                {
                    wallManager.CreateWall(gameMap.HoveredTile);
                }
                else if (args.Button == MouseButton.Right && gameMap.HoveredTile != null)
                {
                    wallManager.RemoveWall(gameMap.HoveredTile);
                }
            };
        }

        public override void Update(GameTime gameTime)
        {
            this.UpdateCamera();
            base.Update(gameTime);
        }

        /// <summary>
        /// Updates the position of the camera according
        /// to the user's input.
        /// </summary>
        private void UpdateCamera()
        {
            var keyboardState = Keyboard.GetState();
            var movementVector = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.D))
            {
                movementVector.X += 1;
            }
            
            if (keyboardState.IsKeyDown(Keys.A))
            {
                movementVector.X -= 1;
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                movementVector.Y += 1;
            }

            if (keyboardState.IsKeyDown(Keys.W))
            {
                movementVector.Y -= 1;
            }

            if (movementVector != Vector2.Zero)
            {
                movementVector.Normalize();
                movementVector *= CameraMovementSpeed;
                this.Camera.Move(movementVector);
            }
        }
    }
}
