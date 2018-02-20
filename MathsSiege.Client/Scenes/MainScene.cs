using MathsSiege.Client.Entities;
using MathsSiege.Client.Framework;
using MathsSiege.Client.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
        private TrapManager trapManager;
        private EnemyManager enemyManager;

        private DefenceMenu defenceMenu;

        private SoundEffect itemPlacedSound;

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
            var backgroundMusic = this.Content.Load<Song>(ContentPaths.Sounds.Background);
            var buttonClickSound = this.Content.Load<SoundEffect>(ContentPaths.Sounds.ButtonClicked);
            var cannon = this.Content.Load<Texture2D>(ContentPaths.Textures.Cannon);
            var itemPlacedSound = this.Content.Load<SoundEffect>(ContentPaths.Sounds.ItemPlaced);
            var map = this.Content.Load<TiledMap>(ContentPaths.TiledMap.Map);
            var spikes = this.Content.Load<Texture2D>(ContentPaths.Textures.SpikesTrap);
            var tileOverlay = this.Content.Load<Texture2D>(ContentPaths.Textures.TileOverlay);
            var wall = this.Content.Load<Texture2D>(ContentPaths.Textures.Wall);
            #endregion

            this.BackgroundImage = background;
            
            var gameMap = new GameMap(map);
            var hoveredTileOverlay = new HoveredTileOverlay(tileOverlay);
            var wallManager = new WallManager(wall);
            var defenceManager = new DefenceManager();
            var trapManager = new TrapManager();
            var enemyManager = new EnemyManager();
            var projectileManager = new ProjectileManager();

            this.Services.AddService(gameMap);
            this.Services.AddService(wallManager);
            this.Services.AddService(defenceManager);
            this.Services.AddService(trapManager);
            this.Services.AddService(enemyManager);
            this.Services.AddService(projectileManager);

            this.AddEntity(gameMap);
            this.AddEntity(hoveredTileOverlay);
            this.AddEntity(wallManager);
            this.AddEntity(defenceManager);
            this.AddEntity(trapManager);
            this.AddEntity(enemyManager);
            this.AddEntity(projectileManager);

            this.gameMap = gameMap;
            this.wallManager = wallManager;
            this.defenceManager = defenceManager;
            this.trapManager = trapManager;
            this.enemyManager = enemyManager;
            this.itemPlacedSound = itemPlacedSound;

            // Center the camera.
            this.Camera.LookAt(Vector2.Zero);

            #region Initialise defence menu
            this.defenceMenu = new DefenceMenu(new Vector2(200, this.GraphicsDevice.Viewport.Height));

            this.defenceMenu.AddItem(DefenceTypes.Wall, wall);
            this.defenceMenu.AddItem(DefenceTypes.Cannon, cannon);
            this.defenceMenu.AddItem(DefenceTypes.Spikes, spikes);

            this.defenceMenu.ItemClicked += () => buttonClickSound.Play();

            this.UserInterface.AddEntity(this.defenceMenu);
            #endregion

            var mouseListener = this.Game.Services.GetService<MouseListener>();

            mouseListener.MouseClicked += this.MouseListener_MouseClicked;

            MediaPlayer.Volume = 0;
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;

            this.stopwatch.Start();
        }

        private void MouseListener_MouseClicked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButton.Left && this.UserInterface.TargetEntity == null
                && this.gameMap.HoveredTile != null)
            {
                if (this.defenceMenu.SelectedItem?.Name == DefenceTypes.Wall)
                {
                    this.wallManager.CreateWall(this.gameMap.HoveredTile);
                    this.itemPlacedSound.Play();
                }
                else if (this.defenceMenu.SelectedItem?.Name == DefenceTypes.Cannon)
                {
                    this.defenceManager.CreateDefence(DefenceTypes.Cannon, this.gameMap.HoveredTile);
                    this.itemPlacedSound.Play();
                }
                else if (this.defenceMenu.SelectedItem?.Name == DefenceTypes.Spikes)
                {
                    this.trapManager.CreateTrap(DefenceTypes.Spikes, this.gameMap.HoveredTile);
                    this.itemPlacedSound.Play();
                }
            }
            else if (e.Button == MouseButton.Right && this.gameMap.HoveredTile != null)
            {
                if (!this.wallManager.RemoveWall(this.gameMap.HoveredTile))
                {
                    if (!this.defenceManager.RemoveDefence(this.gameMap.HoveredTile))
                    {
                        this.trapManager.RemoveTrap(this.gameMap.HoveredTile);
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            this.UpdateCamera();

            if (MediaPlayer.Volume < 1)
            {
                MediaPlayer.Volume = MathHelper.Min(MediaPlayer.Volume + 0.005f, 1);
            }

            if (this.stopwatch.ElapsedMilliseconds > EnemySpawnInterval)
            {
                var i = this.random.Next(this.gameMap.SpawnableTiles.Count);
                var tile = this.gameMap.SpawnableTiles[i];
                this.enemyManager.CreateRandomEnemy(tile);
                this.stopwatch.Restart();
            }

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
