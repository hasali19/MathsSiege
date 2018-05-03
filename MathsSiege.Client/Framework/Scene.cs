using GeonBit.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using System.Linq;

namespace MathsSiege.Client.Framework
{
    public class Scene
    {
        /// <summary>
        /// Whether the scene should be updated or not.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Whether the scene should be drawn or not.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// The global <see cref="Game"/> instance.
        /// </summary>
        public Game Game { get; }

        /// <summary>
        /// The global <see cref="GraphicsDevice"/> instance.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; }

        /// <summary>
        /// The scene's <see cref="ContentManager"/>.
        /// </summary>
        public ContentManager Content { get; }

        /// <summary>
        /// The scene's <see cref="GameServiceContainer"/>.
        /// </summary>
        public GameServiceContainer Services { get; }

        /// <summary>
        /// The scene's <see cref="SpriteBatch"/>.
        /// </summary>
        public SpriteBatch SpriteBatch { get; }

        /// <summary>
        /// The global <see cref="SceneManager"/>.
        /// </summary>
        public SceneManager SceneManager { get; }

        /// <summary>
        /// The scene's <see cref="Camera2D"/>.
        /// </summary>
        public Camera2D Camera { get; }

        /// <summary>
        /// The scene's <see cref="UserInterface"/>.
        /// </summary>
        public UserInterface UserInterface { get; protected set; }

        /// <summary>
        /// The <see cref="RenderTarget2D"/> to which the scene's content is drawn.
        /// </summary>
        public RenderTarget2D RenderTarget { get; private set; }

        /// <summary>
        /// The background color of the scene.
        /// </summary>
        public Color ClearColor { get; set; } = Color.Transparent;

        /// <summary>
        /// The background image for the scene.
        /// </summary>
        public Texture2D BackgroundImage { get; set; }

        private Bag<DrawableEntity> entities = new Bag<DrawableEntity>();

        public Scene(Game game)
        {
            Game = game;
            GraphicsDevice = game.GraphicsDevice;
            Content = new ContentManager(game.Services, game.Content.RootDirectory);
            Services = new GameServiceContainer();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            SceneManager = game.Services.GetService<SceneManager>();
            Camera = new Camera2D(GraphicsDevice);
            UserInterface = new UserInterface();
        }

        public void AddEntity(DrawableEntity entity)
        {
            entity.Scene = this;
            entities.Add(entity);
            entity.OnAddedToScene();
        }

        public void RemoveEntity(DrawableEntity entity)
        {
            entity.Scene = null;
            entities.Remove(entity);
            entity.OnRemovedFromScene();
        }

        /// <summary>
        /// Called when the scene is added to the stack.
        /// </summary>
        public virtual void Initialise()
        {
            UserInterface.Active = UserInterface;

            RenderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }

        /// <summary>
        /// Called when the scene is removed from the stack.
        /// </summary>
        public virtual void Destroy()
        {
            UserInterface.Active = null;

            RenderTarget.Dispose();
        }

        /// <summary>
        /// Called when a new scene is added above this scene.
        /// </summary>
        public virtual void Pause()
        {
            IsActive = false;
            IsVisible = false;

            UserInterface.Active = null;
        }

        /// <summary>
        /// Called when the scene above this scene is removed.
        /// </summary>
        public virtual void Resume()
        {
            IsActive = true;
            IsVisible = true;

            UserInterface.Active = UserInterface;
        }

        /// <summary>
        /// Updates the scene.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            UserInterface.Update(gameTime);

            foreach (var entity in entities.Where(entity => entity.IsActive))
            {
                entity.Update(gameTime);
            }
        }

        /// <summary>
        /// Draws the scene.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Draw(GameTime gameTime)
        {
            if (IsActive && UserInterface.UseRenderTarget)
            {
                UserInterface.Draw(SpriteBatch);
                GraphicsDevice.SetRenderTarget(RenderTarget);
            }

            // Clear the render target.
            GraphicsDevice.Clear(ClearColor);

            // Draw background content.
            SpriteBatch.Begin();
            DrawBackground();
            SpriteBatch.End();

            SpriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: Camera.GetViewMatrix());

            // Draw visible entities.
            foreach (var entity in entities.Where(entity => entity.IsVisible))
            {
                entity.Draw(gameTime);
            }

            SpriteBatch.End();

            // Draw foreground content.
            DrawForeground();
        }

        /// <summary>
        /// Draws content in the background in screen space, before
        /// the scene's entities are drawn.
        /// </summary>
        protected virtual void DrawBackground()
        {
            if (BackgroundImage != null)
            {
                SpriteBatch.Draw(BackgroundImage, GraphicsDevice.Viewport.Bounds, Color.White);
            }
        }

        /// <summary>
        /// Draws content in the foreground in screen space, after
        /// the scene's entities are drawn.
        /// </summary>
        protected virtual void DrawForeground()
        {
            if (IsActive)
            {
                if (UserInterface.UseRenderTarget)
                {
                    UserInterface.DrawMainRenderTarget(SpriteBatch);
                }
                else
                {
                    UserInterface.Draw(SpriteBatch);
                }
            }
        }
    }
}
