﻿using Microsoft.Xna.Framework;
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
