using MathsSiege.Client.Framework;
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
            base.Initialize();

            var scene = new Scene(this) { ClearColor = Color.RoyalBlue };
            this.sceneManager.PushScene(scene);
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
