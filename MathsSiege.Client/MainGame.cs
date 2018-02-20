using GeonBit.UI;
using MathsSiege.Client.Framework;
using MathsSiege.Client.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;

namespace MathsSiege.Client
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SceneManager sceneManager;
        
        public MainGame()
        {
            this.graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
            };

            this.Content.RootDirectory = "Content";

            var mouseListener = new MouseListener();
            var keyboardListener = new KeyboardListener();
            this.sceneManager = new SceneManager(this);

            this.Components.Add(new InputListenerComponent(this, mouseListener, keyboardListener));
            this.Components.Add(this.sceneManager);

            this.Services.AddService(mouseListener);
            this.Services.AddService(keyboardListener);
            this.Services.AddService(this.sceneManager);
        }

        protected override void Initialize()
        {
            UserInterface.Initialize(this.Content, "custom");

            base.Initialize();
            
            this.sceneManager.PushScene(new MainMenuScene(this));
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }
    }
}
