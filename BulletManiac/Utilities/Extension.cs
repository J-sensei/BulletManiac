using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletManiac.Utilities
{
    public static class Extensions
    {
        public static Texture2D CropTexture2D(this Texture2D spriteSheet, Rectangle source)
        {
            Texture2D croppedTexture2d = new Texture2D(spriteSheet.GraphicsDevice, source.Width, source.Height);
            Color[] imageData = new Color[spriteSheet.Width * spriteSheet.Height];
            Color[] croppedImageData = new Color[source.Width * source.Height];

            spriteSheet.GetData<Color>(imageData);

            int index = 0;

            for (int y = source.Y; y < source.Y + source.Height; y++)
            {
                for (int x = source.X; x < source.X + source.Width; x++)
                {
                    croppedImageData[index] = imageData[y * spriteSheet.Width + x];
                    index++;
                }
            }

            croppedTexture2d.SetData<Color>(croppedImageData);
            return croppedTexture2d;
        }
    }
}
