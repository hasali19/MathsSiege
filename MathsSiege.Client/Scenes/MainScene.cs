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
        private UserPreferences preferences;

        private Stopwatch spawnStopwatch = new Stopwatch();
        private Stopwatch questionStopwatch = new Stopwatch();
        private EnemySpawner spawner;

        private GameMap gameMap;
        private WallManager wallManager;
        private DefenceManager defenceManager;
        private TrapManager trapManager;
        private EnemyManager enemyManager;
        private PlayerStats stats;
        private Castle castle;

        private DefenceMenu defenceMenu;
        private StatsView statsView;

        private SoundEffect itemPlacedSound;

        public MainScene(Game game) : base(game)
        {
            ClearColor = Color.White;
            UserInterface.UseRenderTarget = true;

            client = Game.Services.GetService<DataClient>();
            preferences = Game.Services.GetService<UserPreferences>();

            stats = new PlayerStats();
            
            if (Game.Services.GetService<PlayerStats>() != null)
            {
                Game.Services.RemoveService(typeof(PlayerStats));
            }

            Game.Services.AddService(stats);
        }

        public override void Initialise()
        {
            base.Initialise();

            #region Load content
            var background = Content.Load<Texture2D>(ContentPaths.Textures.Background);
            var backgroundMusic = Content.Load<Song>(ContentPaths.Sounds.Background);
            var buttonClickSound = Content.Load<SoundEffect>(ContentPaths.Sounds.ButtonClicked);
            var cannon = Content.Load<Texture2D>(ContentPaths.Textures.Cannon);
            var castleTexture = Content.Load<Texture2D>(ContentPaths.Textures.Castle);
            var itemPlacedSound = Content.Load<SoundEffect>(ContentPaths.Sounds.ItemPlaced);
            var map = Content.Load<TiledMap>(ContentPaths.TiledMap.Map);
            var spikes = Content.Load<Texture2D>(ContentPaths.Textures.SpikesTrap);
            var tileOverlay = Content.Load<Texture2D>(ContentPaths.Textures.TileOverlay);
            var wall = Content.Load<Texture2D>(ContentPaths.Textures.Wall);
            #endregion

            BackgroundImage = background;
            
            var gameMap = new GameMap(map);
            var hoveredTileOverlay = new HoveredTileOverlay(tileOverlay);
            var wallManager = new WallManager(wall);
            var defenceManager = new DefenceManager();
            var trapManager = new TrapManager();
            var enemyManager = new EnemyManager();
            var projectileManager = new ProjectileManager();
            var castle = new Castle(castleTexture) { Position = gameMap[map.Width / 2, map.Height / 2].Position };

            Services.AddService(gameMap);
            Services.AddService(wallManager);
            Services.AddService(defenceManager);
            Services.AddService(trapManager);
            Services.AddService(enemyManager);
            Services.AddService(projectileManager);
            Services.AddService(castle);

            AddEntity(gameMap);
            AddEntity(hoveredTileOverlay);
            AddEntity(wallManager);
            AddEntity(defenceManager);
            AddEntity(trapManager);
            AddEntity(enemyManager);
            AddEntity(projectileManager);
            AddEntity(castle);

            this.gameMap = gameMap;
            this.wallManager = wallManager;
            this.defenceManager = defenceManager;
            this.trapManager = trapManager;
            this.enemyManager = enemyManager;
            this.castle = castle;
            this.itemPlacedSound = itemPlacedSound;

            // Center the camera.
            Camera.LookAt(Vector2.Zero);

            #region Initialise defence menu
            defenceMenu = new DefenceMenu(stats, new Vector2(200, GraphicsDevice.Viewport.Height));

            defenceMenu.AddItem(DefenceTypes.Wall, wall, 20);
            defenceMenu.AddItem(DefenceTypes.Cannon, cannon, 80);
            defenceMenu.AddItem(DefenceTypes.Spikes, spikes, 40);

            defenceMenu.ItemClicked += () => buttonClickSound.Play();

            UserInterface.AddEntity(defenceMenu);
            #endregion

            #region Initialise stats view
            statsView = new StatsView(stats, new Vector2(200, 50));
            UserInterface.AddEntity(statsView);
            statsView.AddPointsButton.OnClick += (e) =>
            {
                e.Disabled = true;
                questionStopwatch.Start();
                SceneManager.PushScene(new QuestionScene(Game));
            };
            #endregion


            if (preferences.IsAudioEnabled)
            {
                MediaPlayer.Volume = 0;
                MediaPlayer.Play(backgroundMusic);
                MediaPlayer.IsRepeating = true;
            }

            spawner = new EnemySpawner(this.gameMap, this.enemyManager);

            spawnStopwatch.Start();
            questionStopwatch.Start();

            this.castle.Destroyed += Castle_Destroyed;

            Game.Exiting += Game_Exiting;
        }

        private void Castle_Destroyed(AttackableEntity obj)
        {
            RemoveEntity(castle);
        }

        public override void Destroy()
        {
            base.Destroy();

            MediaPlayer.Stop();

            Game.Exiting -= Game_Exiting;

            Task.Run(UploadSessionInfo).Wait();
        }

        private void Game_Exiting(object sender, EventArgs e)
        {
            Task.Run(UploadSessionInfo).Wait();
        }

        public override void Pause()
        {
            base.Pause();

            IsVisible = true;

            spawnStopwatch.Stop();
            questionStopwatch.Stop();

            MediaPlayer.Pause();
        }

        public override void Resume()
        {
            base.Resume();

            spawnStopwatch.Start();
            questionStopwatch.Start();

            MediaPlayer.Resume();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateCamera();

            if (MediaPlayer.Volume < 1)
            {
                MediaPlayer.Volume = MathHelper.Min(MediaPlayer.Volume + 0.005f, 1);
            }

            spawner.Update(gameTime);

            if (questionStopwatch.ElapsedMilliseconds > QuestionInterval)
            {
                questionStopwatch.Reset();
                statsView.AddPointsButton.Disabled = false;
            }

            if (InputHandler.IsKeyPressed(Keys.Escape))
            {
                SceneManager.PushScene(new PauseMenuScene(Game));
            }

            if (InputHandler.IsMouseButtonPressed(MouseButton.Left))
            {
                OnLeftMouseButtonPressed();
            }
            else if (InputHandler.IsMouseButtonPressed(MouseButton.Right))
            {
                OnRightMouseButtonPressed();
            }

            if (castle.IsDestroyed && defenceManager.DefenceCount == 0)
            {
                OnGameOver();
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
                Camera.Move(movementVector);
            }
        }

        private void OnLeftMouseButtonPressed()
        {
            if (UserInterface.TargetEntity == null && gameMap.HoveredTile != null
                && defenceMenu.SelectedItem != null)
            {
                var tile = gameMap.HoveredTile;
                if (defenceMenu.SelectedItem.Cost <= stats.Points
                    // Make sure the tile does not contain any objects
                    && !(wallManager.CheckContainsWall(tile, out _)
                    || defenceManager.CheckContainsDefence(tile)
                    || trapManager.CheckContainsTrap(tile)
                    || enemyManager.CheckTileContainsEnemy(tile)
                    || castle.ContainsTile(tile)))
                {
                    if (defenceMenu.SelectedItem.Name == DefenceTypes.Wall)
                    {
                        wallManager.CreateWall(gameMap.HoveredTile);
                    }
                    else if (defenceMenu.SelectedItem.Name == DefenceTypes.Cannon)
                    {
                        defenceManager.CreateDefence(DefenceTypes.Cannon, gameMap.HoveredTile);
                    }
                    else if (defenceMenu.SelectedItem.Name == DefenceTypes.Spikes)
                    {
                        trapManager.CreateTrap(DefenceTypes.Spikes, gameMap.HoveredTile);
                    }

                    stats.Points -= defenceMenu.SelectedItem.Cost;

                    if (preferences.IsAudioEnabled)
                    {
                        itemPlacedSound.Play();
                    }
                }
            }
        }

        private void OnRightMouseButtonPressed()
        {
            if (gameMap.HoveredTile != null
                && !wallManager.RemoveWall(gameMap.HoveredTile)
                && !defenceManager.RemoveDefence(gameMap.HoveredTile))
            {
                trapManager.RemoveTrap(gameMap.HoveredTile);
            }
        }

        private void OnGameOver()
        {
            SceneManager.PushScene(new GameOverScene(Game));
        }

        private Task<bool> UploadSessionInfo()
        {
            var session = new GameSession
            {
                StartTime = DateTime.Now - statsView.ElapsedTime,
                EndTime = DateTime.Now,
                Answers = (ICollection<Answer>)stats.Answers
            };

            return client.PostGameSession(session);
        }
    }
}
