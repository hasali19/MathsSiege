using MathsSiege.Client.Entities;
using MathsSiege.Client.Framework;
using MathsSiege.Client.Gui;
using MathsSiege.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Tiled;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MathsSiege.Client.Scenes
{
    /// <summary>
    /// The main game scene.
    /// </summary>
    public class MainScene : Scene
    {
        private const int CameraMovementSpeed = 10;
        private const int EnemySpawnInterval = 10_000;
        private const int QuestionInterval = 10_000;

        private DataClient client;

        private Stopwatch spawnStopwatch = new Stopwatch();
        private Stopwatch questionStopwatch = new Stopwatch();
        private Random random = new Random();
        private EnemySpawner spawner;

        private GameMap gameMap;
        private WallManager wallManager;
        private DefenceManager defenceManager;
        private TrapManager trapManager;
        private EnemyManager enemyManager;
        private PlayerStats stats;

        private DefenceMenu defenceMenu;
        private StatsView statsView;

        private SoundEffect itemPlacedSound;

        public MainScene(Game game) : base(game)
        {
            this.ClearColor = Color.White;
            this.UserInterface.UseRenderTarget = true;

            this.client = this.Game.Services.GetService<DataClient>();

            this.stats = new PlayerStats();
            
            if (this.Game.Services.GetService<PlayerStats>() != null)
            {
                this.Game.Services.RemoveService(typeof(PlayerStats));
            }

            this.Game.Services.AddService(this.stats);
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
            this.defenceMenu = new DefenceMenu(this.stats, new Vector2(200, this.GraphicsDevice.Viewport.Height));

            this.defenceMenu.AddItem(DefenceTypes.Wall, wall, 50);
            this.defenceMenu.AddItem(DefenceTypes.Cannon, cannon, 50);
            this.defenceMenu.AddItem(DefenceTypes.Spikes, spikes, 50);

            this.defenceMenu.ItemClicked += () => buttonClickSound.Play();

            this.UserInterface.AddEntity(this.defenceMenu);
            #endregion

            #region Initialise stats view
            this.statsView = new StatsView(this.stats, new Vector2(200, 50));
            this.UserInterface.AddEntity(this.statsView);
            this.statsView.AddPointsButton.OnClick += (e) =>
            {
                e.Disabled = true;
                this.questionStopwatch.Start();
                this.SceneManager.PushScene(new QuestionScene(this.Game));
            };
            #endregion

            MediaPlayer.Volume = 0;
            MediaPlayer.Play(backgroundMusic);
            MediaPlayer.IsRepeating = true;

            this.spawner = new EnemySpawner(this.gameMap, this.enemyManager);

            this.spawnStopwatch.Start();
            this.questionStopwatch.Start();

            this.Game.Exiting += this.Game_Exiting;
        }

        public override void Destroy()
        {
            base.Destroy();

            this.Game.Exiting -= this.Game_Exiting;

            Task.Run(this.UploadSessionInfo).Wait();
        }

        private void Game_Exiting(object sender, EventArgs e)
        {
            Task.Run(this.UploadSessionInfo).Wait();
        }

        public override void Pause()
        {
            base.Pause();

            this.IsVisible = true;

            this.spawnStopwatch.Stop();
            this.questionStopwatch.Stop();
        }

        public override void Resume()
        {
            base.Resume();

            this.spawnStopwatch.Start();
            this.questionStopwatch.Start();
        }

        public override void Update(GameTime gameTime)
        {
            this.UpdateCamera();

            if (MediaPlayer.Volume < 1)
            {
                MediaPlayer.Volume = MathHelper.Min(MediaPlayer.Volume + 0.005f, 1);
            }

            this.spawner.Update(gameTime);

            if (this.questionStopwatch.ElapsedMilliseconds > QuestionInterval)
            {
                this.questionStopwatch.Reset();
                this.statsView.AddPointsButton.Disabled = false;
            }

            if (InputHandler.IsKeyPressed(Keys.Escape))
            {
                this.SceneManager.PushScene(new PauseMenuScene(this.Game));
            }

            if (InputHandler.IsMouseButtonPressed(MouseButton.Left))
            {
                this.OnLeftMouseButtonPressed();
            }
            else if (InputHandler.IsMouseButtonPressed(MouseButton.Right))
            {
                this.OnRightMouseButtonPressed();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Updates the position of the camera according
        /// to the user's input.
        /// </summary>
        private void UpdateCamera()
        {
            var movementVector = Vector2.Zero;

            if (InputHandler.IsKeyDown(Keys.D))
            {
                movementVector.X += 1;
            }
            
            if (InputHandler.IsKeyDown(Keys.A))
            {
                movementVector.X -= 1;
            }

            if (InputHandler.IsKeyDown(Keys.S))
            {
                movementVector.Y += 1;
            }

            if (InputHandler.IsKeyDown(Keys.W))
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

        private void OnLeftMouseButtonPressed()
        {
            if (this.UserInterface.TargetEntity == null && this.gameMap.HoveredTile != null
                && this.defenceMenu.SelectedItem != null)
            {
                var tile = this.gameMap.HoveredTile;
                if (this.defenceMenu.SelectedItem.Cost <= this.stats.Points
                    // Make sure the tile does not contain any objects
                    && !(this.wallManager.CheckContainsWall(tile, out _)
                    || this.defenceManager.CheckContainsDefence(tile)
                    || this.trapManager.CheckContainsTrap(tile)
                    || this.enemyManager.CheckTileContainsEnemy(tile)))
                {
                    if (this.defenceMenu.SelectedItem.Name == DefenceTypes.Wall)
                    {
                        this.wallManager.CreateWall(this.gameMap.HoveredTile);
                        this.itemPlacedSound.Play();
                    }
                    else if (this.defenceMenu.SelectedItem.Name == DefenceTypes.Cannon)
                    {
                        this.defenceManager.CreateDefence(DefenceTypes.Cannon, this.gameMap.HoveredTile);
                        this.itemPlacedSound.Play();
                    }
                    else if (this.defenceMenu.SelectedItem.Name == DefenceTypes.Spikes)
                    {
                        this.trapManager.CreateTrap(DefenceTypes.Spikes, this.gameMap.HoveredTile);
                        this.itemPlacedSound.Play();
                    }

                    this.stats.Points -= this.defenceMenu.SelectedItem.Cost;
                }
            }
        }

        private void OnRightMouseButtonPressed()
        {
            if (this.gameMap.HoveredTile != null)
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

        private async Task<bool> UploadSessionInfo()
        {
            var session = new GameSession
            {
                StartTime = DateTime.Now - this.statsView.ElapsedTime,
                EndTime = DateTime.Now,
                Answers = (ICollection<Answer>)this.stats.Answers
            };

            return await this.client.PostGameSession(session);
        }
    }
}
