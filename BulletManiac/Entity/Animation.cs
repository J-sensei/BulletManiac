using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        /// <summary>
        /// Bounding rectangle of each individual frame
        /// </summary>
        private readonly List<Rectangle> bounds = new List<Rectangle>();
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

        /// <summary>
        /// Takes the texture, number of frames (X and Y), time between frames and which row of the sprite to animate
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="frameCountX"></param>
        /// <param name="frameCountY"></param>
        /// <param name="frameTime"></param>
        /// <param name="row"></param>
        public Animation(Texture2D texture, int frameCountX, int frameCountY, float frameTime, int row = 1)
        {
            this.texture = texture;
            this.frameCount = frameCountX;
            this.frameTime = frameTime;
            frameTimeLeft = frameTime;

            // Calculate rectangle bounds of each frame
            var width = texture.Width / frameCountX;
            var height = texture.Height / frameCountY;

            for(int i = 0; i < frameCount; i++)
            {
                bounds.Add(new Rectangle(i * width, (row - 1) * height, width, height));
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
                frameTimeLeft = frameTime; // reset the frame time
                currentFrame = (currentFrame + 1) % frameCount; // move to next frame
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
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects, float layerDepth)
        {
            spriteBatch.Draw(texture, position, bounds[currentFrame], color, rotation, origin, scale, spriteEffects, 0f);
        }

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
        }
    }
}
