using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BulletManiac.Entity
{
    /// <summary>
    /// Animation for a single sprite sheet
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Sprite sheet
        /// </summary>
        private readonly Texture2D texture;

        private readonly int frameCount;
        /// <summary>
        /// Index of current frmae
        /// </summary>
        private int currentFrame;
        /// <summary>
        /// How much it takes to change frame
        /// </summary>
        private readonly float frameTime;
        private float frameTimeLeft;
        /// <summary>
        /// Switch the animation on / off
        /// </summary>
        private bool active = true;
        private bool looping;

        /// <summary>
        /// Cropped textures based on the bounds data
        /// </summary>
        private Texture2D[] croppedTextures;
        /// <summary>
        /// Get the current frame sprite
        /// </summary>
        public Texture2D CurrentTexture
        {
            get
            {
                return croppedTextures[currentFrame];
            }
        }

        /// <summary>
        /// Animation finish playing
        /// </summary>
        public bool Finish { get; private set; }
        /// <summary>
        /// Play the animation in reverse order
        /// </summary>
        public bool Reverse { get; private set; } = false;

        /// <summary>
        /// Takes the texture, number of frames (X and Y), time between frames and which row of the sprite to animate
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="frameCountX"></param>
        /// <param name="frameCountY"></param>
        /// <param name="frameTime"></param>
        /// <param name="row"></param>
        public Animation(Texture2D texture, int frameCountX, int frameCountY, float frameTime, int row = 1, bool looping = true)
        {
            this.texture = texture;
            this.frameCount = frameCountX;
            this.frameTime = frameTime;
            frameTimeLeft = frameTime;
            this.looping = looping;

            // Calculate rectangle bounds of each frame
            var width = texture.Width / frameCountX;
            var height = texture.Height / frameCountY;

            // Crop the sprite sheet and store into the texture array
            croppedTextures = new Texture2D[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                Rectangle bound = new Rectangle(i * width, (row - 1) * height, width, height);
                croppedTextures[i] = Extensions.CropTexture2D(texture, bound);
            }
        }

        /// <summary>
        /// Update the current frame of animation
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (!active) return;
            float totalSeconds = GameManager.DeltaTime;
            frameTimeLeft -= totalSeconds;

            if(frameTimeLeft <= 0)
            {
                if (looping)
                {
                    LoopingAnimation();
                }
                else
                {
                    NonLoopingAnimation();
                }
            }
        }

        private void LoopingAnimation()
        {
            if (Reverse)
            {
                // Reverse animation frmae
                if(currentFrame == 0)
                {
                    currentFrame = frameCount - 1;
                }
                else
                {
                    currentFrame--;
                }
            }
            else
            {
                currentFrame = (currentFrame + 1) % frameCount; // move to next frame
            }

            frameTimeLeft = frameTime; // reset the frame time
            Finish = false; // Looping animation will never finish
        }

        private void NonLoopingAnimation()
        {
            // Console.WriteLine(currentFrame + " " + frameCount);
            if (currentFrame < frameCount - 1)
            {
                frameTimeLeft = frameTime; // reset the frame time

                currentFrame++;// move to next frame
                Finish = false;
            }
            else
            {
                Finish = true;
            }
        }

        /// <summary>
        /// Draw the correct animation frame
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="rotation"></param>
        /// <param name="origin"></param>
        /// <param name="scale"></param>
        /// <param name="spriteEffects"></param>
        /// <param name="layerDepth"></param>
        //public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects, float layerDepth)
        //{
        //    spriteBatch.Draw(texture, position, bounds[currentFrame], color, rotation, origin, scale, spriteEffects, 0f);
        //}

        /// <summary>
        /// Start the animation playing
        /// </summary>
        public void Start() => active = true;
        /// <summary>
        /// Stop the animation playing
        /// </summary>
        public void Stop() => active = false;

        // Reset the animation
        public void Reset()
        {
            currentFrame = 0;
            frameTimeLeft = frameTime;
            Finish = false;
        }

        /// <summary>
        /// Set the animation reverse
        /// </summary>
        /// <param name="reverse"></param>
        public void SetReverse(bool reverse)
        {
            if (Reverse != reverse) // Only set the reverse and reset the animation when the setting is different
            {
                Reverse = reverse;
                Reset();

                if (reverse)
                {
                    currentFrame = frameCount - 1;
                }
                else
                {
                    currentFrame = 0;
                }
            }
        }
    }
}
