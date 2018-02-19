using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MathsSiege.Client
{
    public static class SpriteBatchExtensions
    {
        private static Texture2D texture;

        private static Texture2D GetTexture(SpriteBatch spriteBatch)
        {
            if (texture == null)
            {
                texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                texture.SetData(new[] { Color.White });
            }

            return texture;
        }

        /// <summary>
        /// Draws a rectangle with a solid fill color.
        /// </summary>
        /// <param name="rectangle">The rectangle to draw.</param>
        /// <param name="fill">The fill color.</param>
        public static void FillRectangle(this SpriteBatch spriteBatch, RectangleF rectangle, Color fill, float depth)
        {
            spriteBatch.Draw(GetTexture(spriteBatch), rectangle.ToRectangle(), null, fill, 0, Vector2.Zero, SpriteEffects.None, depth);
        }

        /// <summary>
        /// Draws a rectangle outline only.
        /// </summary>
        /// <param name="rectangle">The rectangle to draw.</param>
        /// <param name="color">The color of the outline.</param>
        /// <param name="thickness">The thickness of the outline.</param>
        public static void DrawRectangle(this SpriteBatch spriteBatch, RectangleF rectangle, Color color, float depth,
            float thickness = 1.0f)
        {
            var topLeft = new Vector2(rectangle.X, rectangle.Y);
            var topRight = new Vector2(rectangle.Right - thickness, rectangle.Y);
            var bottomLeft = new Vector2(rectangle.X, rectangle.Bottom - thickness);
            var horizontalScale = new Vector2(rectangle.Width, thickness);
            var verticalScale = new Vector2(thickness, rectangle.Height);

            spriteBatch.Draw(GetTexture(spriteBatch), topLeft, null, color, 0, Vector2.Zero, horizontalScale, SpriteEffects.None, depth);
            spriteBatch.Draw(GetTexture(spriteBatch), topLeft, null, color, 0, Vector2.Zero, verticalScale, SpriteEffects.None, depth);
            spriteBatch.Draw(GetTexture(spriteBatch), topRight, null, color, 0, Vector2.Zero, verticalScale, SpriteEffects.None, depth);
            spriteBatch.Draw(GetTexture(spriteBatch), bottomLeft, null, color, 0, Vector2.Zero, horizontalScale, SpriteEffects.None, depth);
        }
    }
}
