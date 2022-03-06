using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteJumper
{
    internal static class ScreenFiller
    {
        private static Texture2D pixel;

        private static void CreateThePixel(SpriteBatch spriteBatch)
        {
            pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
        }

        public static void FillRectangle(this SpriteBatch spriteBatch, Rectangle rect, Color color, float angle)
        {
            if (pixel == null)
            {
                CreateThePixel(spriteBatch);
            }

            spriteBatch.Draw(pixel, rect, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}