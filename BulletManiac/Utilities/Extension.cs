using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Utilities
{
    public static class Extensions
    {
        private static Random random = new Random();
        public static Random Random { get { return random; } }

        public static float RandomRangeFloat(float min, float max)
        {
            double val = (random.NextDouble() * (max - min) + min);
            return (float)val;
        }

        public static float NextFloat()
        {
            return (float)random.NextDouble();
        }

        /// <summary>
        /// From Steering Behavior Lab (Game Algorithm)
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static bool Approximately(float a, float b)
        {
            return (double)MathF.Abs(b - a) < (double)MathF.Max(1E-06f * MathF.Max(MathF.Abs(a),
                MathF.Abs(b)), float.Epsilon * 8.0f);
        }

        /// <summary>
        /// From Flock Lab (Game Algorithm)
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static Vector2 Truncate(Vector2 vector, float maxLength)
        {
            float sqrmag = vector.LengthSquared();
            if (sqrmag > maxLength * maxLength)
            {
                float mag = (float)Math.Sqrt(sqrmag);
                //these intermediate variables force the intermediate result to be
                //of float precision. without this, the intermediate result can be of higher
                //precision, which changes behavior.
                float normalized_x = vector.X / mag;
                float normalized_y = vector.Y / mag;
                return new Vector2(normalized_x * maxLength,
                    normalized_y * maxLength);
            }
            return vector;
        }

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
