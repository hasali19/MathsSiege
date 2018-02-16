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
            this.Game = game;
            this.GraphicsDevice = game.GraphicsDevice;
            this.Content = new ContentManager(game.Services, game.Content.RootDirectory);
            this.Services = new GameServiceContainer();
            this.SpriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.SceneManager = game.Services.GetService<SceneManager>();
            this.Camera = new Camera2D(this.GraphicsDevice);
        }

        public void AddEntity(DrawableEntity entity)
        {
            entity.Scene = this;
            this.entities.Add(entity);
            entity.OnAddedToScene();
        }

        public void RemoveEntity(DrawableEntity entity)
        {
            entity.Scene = null;
            this.entities.Remove(entity);
            entity.OnRemovedFromScene();
        }

        /// <summary>
        /// Called when the scene is added to the stack.
        /// </summary>
        public virtual void Initialise()
        {
            this.RenderTarget = new RenderTarget2D(
                this.GraphicsDevice,
                this.GraphicsDevice.PresentationParameters.BackBufferWidth,
                this.GraphicsDevice.PresentationParameters.BackBufferHeight,
                false,
                this.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24);
        }

        /// <summary>
        /// Called when the scene is removed from the stack.
        /// </summary>
        public virtual void Destroy()
        {
            this.RenderTarget.Dispose();
        }

        /// <summary>
        /// Called when a new scene is added above this scene.
        /// </summary>
        public virtual void Pause()
        {
            this.IsActive = false;
            this.IsVisible = false;
        }

        /// <summary>
        /// Called when the scene above this scene is removed.
        /// </summary>
        public virtual void Resume()
        {
            this.IsActive = true;
            this.IsVisible = true;
        }

        /// <summary>
        /// Updates the scene.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
            foreach (var entity in this.entities.Where(entity => entity.IsActive))
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
            // Clear the render target.
            this.GraphicsDevice.Clear(this.ClearColor);

            // Draw background content.
            this.SpriteBatch.Begin();
            this.DrawBackground();
            this.SpriteBatch.End();

            this.SpriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: this.Camera.GetViewMatrix());

            // Draw visible entities.
            foreach (var entity in this.entities.Where(entity => entity.IsVisible))
            {
                entity.Draw(gameTime);
            }

            this.SpriteBatch.End();

            // Draw foreground content.
            this.SpriteBatch.Begin();
            this.DrawForeground();
            this.SpriteBatch.End();
        }

        /// <summary>
        /// Draws content in the background in screen space, before
        /// the scene's entities are drawn.
        /// </summary>
        protected virtual void DrawBackground()
        {
            if (this.BackgroundImage != null)
            {
                this.SpriteBatch.Draw(this.BackgroundImage, this.GraphicsDevice.Viewport.Bounds, Color.White);
            }
        }

        /// <summary>
        /// Draws content in the foreground in screen space, after
        /// the scene's entities are drawn.
        /// </summary>
        protected virtual void DrawForeground()
        {
        }
    }
}
