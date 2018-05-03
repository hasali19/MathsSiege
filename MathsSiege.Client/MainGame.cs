using GeonBit.UI;
using MathsSiege.Client.Framework;
using MathsSiege.Client.Scenes;
using Microsoft.Xna.Framework;

namespace MathsSiege.Client
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SceneManager sceneManager;
        
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
            };

            Content.RootDirectory = "Content";

            sceneManager = new SceneManager(this);
            Components.Add(sceneManager);
            Services.AddService(sceneManager);
        }

        protected override void Initialize()
        {
            UserInterface.Initialize(Content, "custom");

            var preferences = new UserPreferences();
            preferences.Load();

            if (preferences.IsWindowFullScreen)
            {
                graphics.IsFullScreen = true;
                graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
                graphics.ApplyChanges();
            }

            var dataClient = new DataClient(preferences);

            Services.AddService(preferences);
            Services.AddService(dataClient);

            base.Initialize();

            sceneManager.PushScene(new SplashScene(this));
        }

        protected override void Update(GameTime gameTime)
        {
            InputHandler.Update();

            base.Update(gameTime);
        }
    }
}
