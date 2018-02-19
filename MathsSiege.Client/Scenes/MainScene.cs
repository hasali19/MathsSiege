using MathsSiege.Client.Entities;
using MathsSiege.Client.Framework;
using MathsSiege.Client.Gui;
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
            this.UserInterface.UseRenderTarget = true;
        }

        public override void Initialise()
        {
            base.Initialise();

            #region Load content
            var background = this.Content.Load<Texture2D>(ContentPaths.Textures.Background);
            var cannon = this.Content.Load<Texture2D>(ContentPaths.Textures.Cannon);
            var map = this.Content.Load<TiledMap>(ContentPaths.TiledMap.Map);
            var tileOverlay = this.Content.Load<Texture2D>(ContentPaths.Textures.TileOverlay);
            var wall = this.Content.Load<Texture2D>(ContentPaths.Textures.Wall);
            #endregion

            this.BackgroundImage = background;
            
            var gameMap = new GameMap(map);
            var hoveredTileOverlay = new HoveredTileOverlay(tileOverlay);
            var wallManager = new WallManager(wall);
            var defenceManager = new DefenceManager();
            var enemyManager = new EnemyManager();
            var projectileManager = new ProjectileManager();

            this.Services.AddService(gameMap);
            this.Services.AddService(wallManager);
            this.Services.AddService(defenceManager);
            this.Services.AddService(enemyManager);
            this.Services.AddService(projectileManager);

            this.AddEntity(gameMap);
            this.AddEntity(hoveredTileOverlay);
            this.AddEntity(wallManager);
            this.AddEntity(defenceManager);
            this.AddEntity(enemyManager);
            this.AddEntity(projectileManager);

            // Center the camera.
            this.Camera.LookAt(Vector2.Zero);

            #region Initialise defence menu
            var menu = new DefenceMenu(new Vector2(200, this.GraphicsDevice.Viewport.Height));

            menu.AddItem(DefenceTypes.Wall, wall);
            menu.AddItem(DefenceTypes.Cannon, cannon);

            this.UserInterface.AddEntity(menu);
            #endregion

            enemyManager.CreateRandomEnemy(gameMap[2, 2]);
            enemyManager.CreateRandomEnemy(gameMap[18, 3]);
            enemyManager.CreateRandomEnemy(gameMap[3, 18]);
            enemyManager.CreateRandomEnemy(gameMap[2, 14]);
            enemyManager.CreateRandomEnemy(gameMap[14, 2]);

            var mouseListener = this.Game.Services.GetService<MouseListener>();

            mouseListener.MouseClicked += (sender, args) =>
            {
                if (args.Button == MouseButton.Left && gameMap.HoveredTile != null)
                {
                    if (menu.SelectedItem?.Name == DefenceTypes.Wall)
                    {
                        wallManager.CreateWall(gameMap.HoveredTile);
                    }
                    else if (menu.SelectedItem?.Name == DefenceTypes.Cannon)
                    {
                        defenceManager.CreateDefence(DefenceTypes.Cannon, gameMap.HoveredTile);
                    }
                }
                else if (args.Button == MouseButton.Right && gameMap.HoveredTile != null)
                {
                    if (!wallManager.RemoveWall(gameMap.HoveredTile))
                    {
                        defenceManager.RemoveDefence(gameMap.HoveredTile);
                    }
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
