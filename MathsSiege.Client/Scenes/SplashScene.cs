using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Diagnostics;

namespace MathsSiege.Client.Scenes
{
    public class SplashScene : Scene
    {
        private const string Title = "MathsSiege";

        private const float EnterDuration = 2000;
        private const float IdleDuration = 1000;
        private const float ExitDuration = 2000;

        private SpriteFont font;

        private Vector2 titlePosition;

        private Color titleColor;
        private Color overlayColor = Color.Transparent;

        private Stopwatch stopwatch = new Stopwatch();

        public SplashScene(Game game) : base(game)
        {
        }

        public override void Initialise()
        {
            base.Initialise();

            BackgroundImage = Content.Load<Texture2D>(ContentPaths.Textures.SplashBackground);
            font = Content.Load<SpriteFont>(ContentPaths.Fonts.gravedigger_42);

            titlePosition = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height) / 2
                - font.MeasureString(Title) / 2;

            stopwatch.Start();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (stopwatch.ElapsedMilliseconds < EnterDuration)
            {
                // Fade in text and overlay, increasing alpha between 0 and 1 following a sine curve.
                var alpha = (float)Math.Sin((stopwatch.ElapsedMilliseconds / EnterDuration) * MathHelper.PiOver2);
                titleColor = Color.White * alpha;
                overlayColor = Color.Black * 0.5f * alpha;
            }
            else if (stopwatch.ElapsedMilliseconds >= EnterDuration
                && stopwatch.ElapsedMilliseconds <= EnterDuration + IdleDuration)
            {
                titleColor = Color.White;
                overlayColor = Color.Black * 0.5f;
            }
            else if (stopwatch.ElapsedMilliseconds > EnterDuration + IdleDuration
                && stopwatch.ElapsedMilliseconds < EnterDuration + IdleDuration + ExitDuration)
            {
                // Fade out text and overlay, decreasing alpha between 1 and 0 following a cosine curve.
                var alpha = (float)Math.Cos(((stopwatch.ElapsedMilliseconds - EnterDuration - IdleDuration) / ExitDuration) * MathHelper.PiOver2);
                titleColor = Color.White * alpha;
                overlayColor = Color.Black * 0.5f * alpha;
            }
            else
            {
                SceneManager.ReplaceScene(new MainMenuScene(Game));
            }
        }

        protected override void DrawForeground()
        {
            base.DrawForeground();

            SpriteBatch.Begin();

            SpriteBatch.FillRectangle(GraphicsDevice.Viewport.Bounds, overlayColor);
            SpriteBatch.DrawString(font, Title, titlePosition, titleColor);

            SpriteBatch.End();
        }
    }
}
