using MathsSiege.Client.Entities;
using MathsSiege.Client.Framework;
using MathsSiege.Client.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Tiled;
using System;
using System.Diagnostics;

namespace MathsSiege.Client.Scenes
{
    /// <summary>
    /// The main game scene.
    /// </summary>
    public class MainScene : Scene
    {
        private const int CameraMovementSpeed = 10;
        private const int EnemySpawnInterval = 10_000;

        private Stopwatch stopwatch = new Stopwatch();
        private Random random = new Random();

        private GameMap gameMap;
        private WallManager wallManager;
        private DefenceManager defenceManager;
        private EnemyManager enemyManager;

        private DefenceMenu defenceMenu;

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

            this.gameMap = gameMap;
            this.wallManager = wallManager;
            this.defenceManager = defenceManager;
            this.enemyManager = enemyManager;

            // Center the camera.
            this.Camera.LookAt(Vector2.Zero);

            #region Initialise defence menu
            this.defenceMenu = new DefenceMenu(new Vector2(200, this.GraphicsDevice.Viewport.Height));

            this.defenceMenu.AddItem(DefenceTypes.Wall, wall);
            this.defenceMenu.AddItem(DefenceTypes.Cannon, cannon);

            this.UserInterface.AddEntity(this.defenceMenu);
            #endregion

            var mouseListener = this.Game.Services.GetService<MouseListener>();

            mouseListener.MouseClicked += this.MouseListener_MouseClicked;

            this.stopwatch.Start();
        }

        private void MouseListener_MouseClicked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButton.Left && this.gameMap.HoveredTile != null)
            {
                if (this.defenceMenu.SelectedItem?.Name == DefenceTypes.Wall)
                {
                    this.wallManager.CreateWall(this.gameMap.HoveredTile);
                }
                else if (this.defenceMenu.SelectedItem?.Name == DefenceTypes.Cannon)
                {
                    this.defenceManager.CreateDefence(DefenceTypes.Cannon, this.gameMap.HoveredTile);
                }
            }
            else if (e.Button == MouseButton.Right && this.gameMap.HoveredTile != null)
            {
                if (!this.wallManager.RemoveWall(this.gameMap.HoveredTile))
                {
                    this.defenceManager.RemoveDefence(this.gameMap.HoveredTile);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            this.UpdateCamera();
            base.Update(gameTime);

            if (this.stopwatch.ElapsedMilliseconds > EnemySpawnInterval)
            {
                var i = this.random.Next(this.gameMap.SpawnableTiles.Count);
                var tile = this.gameMap.SpawnableTiles[i];
                this.enemyManager.CreateRandomEnemy(tile);
                this.stopwatch.Restart();
            }
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
