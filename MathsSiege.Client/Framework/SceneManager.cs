using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace MathsSiege.Client.Framework
{
    public class SceneManager : DrawableGameComponent
    {
        private IList<Scene> scenes = new List<Scene>();

        private SpriteBatch spriteBatch;

        public SceneManager(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
        }

        /// <summary>
        /// Gets the current topmost scene on the stack.
        /// </summary>
        public Scene CurrentScene => this.scenes.LastOrDefault();

        /// <summary>
        /// Pushes a new scene onto the stack.
        /// </summary>
        /// <param name="scene"></param>
        public void PushScene(Scene scene)
        {
            this.CurrentScene?.Pause();
            this.scenes.Add(scene);
            scene.Initialise();
        }

        /// <summary>
        /// Remove the topmost scene from the stack.
        /// </summary>
        public void PopScene()
        {
            if (this.scenes.Count > 0)
            {
                this.CurrentScene.Destroy();
                this.scenes.RemoveAt(this.scenes.Count - 1);
                this.CurrentScene?.Resume();
            }
        }

        /// <summary>
        /// Replaces the topmost scene with a new scene.
        /// </summary>
        /// <param name="scene"></param>
        public void ReplaceScene(Scene scene)
        {
            if (this.scenes.Count > 0)
            {
                this.CurrentScene.Destroy();
                this.scenes.RemoveAt(this.scenes.Count - 1);
                this.scenes.Add(scene);
                scene.Initialise();
            }
        }

        /// <summary>
        /// Clears all current scenes from the stack, then adds a new scene.
        /// </summary>
        /// <param name="scene"></param>
        public void Clear(Scene scene)
        {
            foreach (var s in this.scenes)
            {
                s.Destroy();
            }

            this.scenes.Clear();
            this.scenes.Add(scene);
            scene.Initialise();
        }

        /// <summary>
        /// Updates all active scenes.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            for (int i = this.scenes.Count - 1; i >= 0; i--)
            {
                var scene = this.scenes[i];
                if (scene.IsActive)
                {
                    scene.Update(gameTime);
                }
            }
        }

        /// <summary>
        /// Draws all visible scenes.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // Draw scenes to their respective render targets.
            for (int i = 0; i < this.scenes.Count; i++)
            {
                var scene = this.scenes[i];
                if (scene.IsVisible)
                {
                    scene.GraphicsDevice.SetRenderTarget(scene.RenderTarget);
                    scene.Draw(gameTime);
                }
            }

            // Clear the screen.
            this.GraphicsDevice.SetRenderTarget(null);
            this.GraphicsDevice.Clear(Color.Black);

            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            // Draw all the render targets to the back buffer.
            for (int i = 0; i < this.scenes.Count; i++)
            {
                var scene = this.scenes[i];
                if (scene.IsVisible)
                {
                    this.spriteBatch.Draw(scene.RenderTarget, this.GraphicsDevice.Viewport.Bounds, Color.White);
                }
            }

            this.spriteBatch.End();
        }
    }
}
