using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BulletManiac.SpriteAnimation
{
    /// <summary>
    /// Animation for a single sprite sheet
    /// </summary>
    public class Animation : IDisposable
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
        /// UVs for the textures
        /// </summary>
        private readonly Rectangle[] bounds;

        /// <summary>
        /// Current frame index in the animation (start from 0)
        /// </summary>
        public int CurrentFrameIndex
        {
            get { return currentFrame; }
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
        /// Get the bound of the animation texture
        /// </summary>
        public Vector2 TextureBound
        {
            get
            {
                return new Vector2(bounds[0].Width, bounds[0].Height);
            }
        }

        /// <summary>
        /// Load and Pre calculated animation
        /// </summary>
        /// <param name="resources"></param>
        public static void LoadAnimations()
        {
            ResourcesManager.LoadAnimation("DefaultBullet_Animation", new Animation(ResourcesManager.FindTexture("Bullet1"), 5, 25, 0.1f, 6));
            ResourcesManager.LoadAnimation("Walking_Smoke_Animation", new Animation(ResourcesManager.FindTexture("Walking_Smoke"), 6, 1, 0.1f, looping: false));
            ResourcesManager.LoadAnimation("Destroy_Smoke_Animation", new Animation(ResourcesManager.FindTexture("Destroy_Smoke"), 5, 1, 0.1f, looping: false));
            ResourcesManager.LoadAnimation("Spawn_Smoke_Animation", new Animation(ResourcesManager.FindTexture("Spawn_Smoke"), 8, 1, 0.1f, looping: false));

            ResourcesManager.LoadAnimation("TrackBullet_Animation", new Animation(ResourcesManager.FindTexture("Bullet1"), 5, 25, 0.1f, 5));
            ResourcesManager.LoadAnimation("ExplosionBullet_Animation", new Animation(ResourcesManager.FindTexture("Bullet1"), 5, 25, 0.1f, 16));

            ResourcesManager.LoadTexture("Bullet_Explosion", "SpriteSheet/Effect/Bullet_Explosion");
            ResourcesManager.LoadAnimation("BulletExplode_Animation", new Animation(ResourcesManager.FindTexture("Bullet_Explosion"), 8, 1, 0.07f, looping: false));
        }

        public Animation(Animation animation)
        {
            texture = animation.texture;
            frameCount = animation.frameCount;
            frameTime = animation.frameTime;
            frameTimeLeft = animation.frameTime;
            looping = animation.looping;
            bounds = animation.bounds; // Skip calculation for the bounds
        }

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
            frameCount = frameCountX;
            this.frameTime = frameTime;
            frameTimeLeft = frameTime;
            this.looping = looping;

            // Calculate rectangle bounds of each frame
            var width = texture.Width / frameCountX;
            var height = texture.Height / frameCountY;

            // Calculate UV coordinate for each frame
            bounds = new Rectangle[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                Rectangle bound = new Rectangle(i * width, (row - 1) * height, width, height);
                bounds[i] = bound;
            }
        }

        public Animation(Texture2D texture, int frameCountX, int width, int height, float frameTime, int row = 1, bool looping = true)
        {
            this.texture = texture;
            frameCount = frameCountX;
            this.frameTime = frameTime;
            frameTimeLeft = frameTime;
            this.looping = looping;

            // Calculate UV coordinate for each frame
            bounds = new Rectangle[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                Rectangle bound = new Rectangle(i * width, (row - 1) * height, width, height);
                bounds[i] = bound;
            }
        }

        /// <summary>
        /// Update the current frame of animation
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (!active) return;
            float totalSeconds = Time.DeltaTime;
            frameTimeLeft -= totalSeconds;

            if (frameTimeLeft <= 0)
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
                if (currentFrame == 0)
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
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects spriteEffects, float layerDepth)
        {
            spriteBatch.Draw(texture, position, bounds[currentFrame], color, rotation, origin, scale, spriteEffects, layerDepth);
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

        public void Dispose()
        {

        }
    }
}
