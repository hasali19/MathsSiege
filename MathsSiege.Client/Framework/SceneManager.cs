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

        private bool shouldClearScenes = false;

        public SceneManager(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        /// <summary>
        /// Gets the current topmost scene on the stack.
        /// </summary>
        public Scene CurrentScene => scenes.LastOrDefault();

        /// <summary>
        /// Pushes a new scene onto the stack.
        /// </summary>
        /// <param name="scene"></param>
        public void PushScene(Scene scene)
        {
            CurrentScene?.Pause();
            scenes.Add(scene);
            scene.Initialise();
        }

        /// <summary>
        /// Remove the topmost scene from the stack.
        /// </summary>
        public void PopScene()
        {
            if (scenes.Count > 0)
            {
                CurrentScene.Destroy();
                scenes.RemoveAt(scenes.Count - 1);
                CurrentScene?.Resume();
            }
        }

        /// <summary>
        /// Replaces the topmost scene with a new scene.
        /// </summary>
        /// <param name="scene"></param>
        public void ReplaceScene(Scene scene)
        {
            if (scenes.Count > 0)
            {
                CurrentScene.Destroy();
                scenes.RemoveAt(scenes.Count - 1);
                scenes.Add(scene);
                scene.Initialise();
            }
        }

        /// <summary>
        /// Clears all current scenes from the stack, then adds a new scene.
        /// </summary>
        /// <param name="scene"></param>
        public void Clear(Scene scene)
        {
            foreach (var s in scenes)
            {
                s.Destroy();
            }

            shouldClearScenes = true;
            scenes.Add(scene);
            scene.Initialise();
        }

        /// <summary>
        /// Updates all active scenes.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            for (int i = scenes.Count - 1; i >= 0; i--)
            {
                var scene = scenes[i];
                if (scene.IsActive)
                {
                    scene.Update(gameTime);
                }
            }

            if (shouldClearScenes)
            {
                while (scenes.Count > 1)
                {
                    scenes.RemoveAt(0);
                }

                shouldClearScenes = false;
            }
        }

        /// <summary>
        /// Draws all visible scenes.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // Draw scenes to their respective render targets.
            for (int i = 0; i < scenes.Count; i++)
            {
                var scene = scenes[i];
                if (scene.IsVisible)
                {
                    scene.GraphicsDevice.SetRenderTarget(scene.RenderTarget);
                    scene.Draw(gameTime);
                }
            }

            // Clear the screen.
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            // Draw all the render targets to the back buffer.
            for (int i = 0; i < scenes.Count; i++)
            {
                var scene = scenes[i];
                if (scene.IsVisible)
                {
                    spriteBatch.Draw(scene.RenderTarget, GraphicsDevice.Viewport.Bounds, Color.White);
                }
            }

            spriteBatch.End();
        }
    }
}
