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

        private const float EnterDuration = 3000;
        private const float IdleDuration = 1000;
        private const float ExitDuration = 3000;

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

            this.BackgroundImage = this.Content.Load<Texture2D>(ContentPaths.Textures.SplashBackground);
            this.font = this.Content.Load<SpriteFont>(ContentPaths.Fonts.gravedigger_42);

            this.titlePosition = new Vector2(this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height) / 2
                - this.font.MeasureString(Title) / 2;

            this.stopwatch.Start();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.stopwatch.ElapsedMilliseconds < EnterDuration)
            {
                // Fade in text and overlay, increasing alpha between 0 and 1 following a sine curve.
                var alpha = (float)Math.Sin((this.stopwatch.ElapsedMilliseconds / EnterDuration) * MathHelper.PiOver2);
                this.titleColor = Color.White * alpha;
                this.overlayColor = Color.Black * 0.5f * alpha;
            }
            else if (this.stopwatch.ElapsedMilliseconds >= EnterDuration
                && this.stopwatch.ElapsedMilliseconds <= EnterDuration + IdleDuration)
            {
                this.titleColor = Color.White;
                this.overlayColor = Color.Black * 0.5f;
            }
            else if (this.stopwatch.ElapsedMilliseconds > EnterDuration + IdleDuration
                && this.stopwatch.ElapsedMilliseconds < EnterDuration + IdleDuration + ExitDuration)
            {
                // Fade out text and overlay, decreasing alpha between 1 and 0 following a cosine curve.
                var alpha = (float)Math.Cos(((this.stopwatch.ElapsedMilliseconds - EnterDuration - IdleDuration) / ExitDuration) * MathHelper.PiOver2);
                this.titleColor = Color.White * alpha;
                this.overlayColor = Color.Black * 0.5f * alpha;
            }
            else
            {
                this.SceneManager.ReplaceScene(new MainMenuScene(this.Game));
            }
        }

        protected override void DrawForeground()
        {
            base.DrawForeground();

            this.SpriteBatch.Begin();

            this.SpriteBatch.FillRectangle(this.GraphicsDevice.Viewport.Bounds, this.overlayColor);
            this.SpriteBatch.DrawString(this.font, Title, this.titlePosition, this.titleColor);

            this.SpriteBatch.End();
        }
    }
}
