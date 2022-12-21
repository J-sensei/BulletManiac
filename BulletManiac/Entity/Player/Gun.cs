using BulletManiac.Entity.Bullet;
using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BulletManiac.Entity.Player
{
    public class Gun : GameObject
    {
        private Animation animation;
        private GameObject holder; // Holder of this gun

        private bool shooting = false;

        const float MIN_SHOOT_ANIMATION_SPEED = 0.01f;
        const float MAX_SHOOT_ANIMATION_SPEED = 0.05f;
        float currentShootAnimationSpeed = MIN_SHOOT_ANIMATION_SPEED; // Test

        const float DEFAULT_ACCURACY = 0.25f;
        float accuracy = DEFAULT_ACCURACY; // The lower the number, the better accuracy

        /// <summary>
        /// True need to render infront of the holder
        /// </summary>
        public bool RenderInfront { get; private set; }

        public Gun(GameObject holder)
        {
            name = "Player Gun";
            scale = new Vector2(0.4f);
            this.holder = holder;

            animation = new Animation(GameManager.Resources.FindTexture("Player_Pistol"), 12, 1, currentShootAnimationSpeed, looping: false);
            animation.Stop();
            texture = animation.CurrentTexture;
            origin = new Vector2(0f, texture.Bounds.Center.ToVector2().Y);
        }

        protected override Rectangle CalculateBound()
        {
            return Rectangle.Empty;
        }

        public override void Initialize()
        {
            spriteEffects = SpriteEffects.None;
            base.Initialize();
        }

        Vector2 offset = new Vector2(0f);
        public override void Update(GameTime gameTime)
        {
            animation.Update(gameTime);
            texture = animation.CurrentTexture;

            Shoot(); // Gun shooting
            // If the gun is shooting, the animation need to play
            if (shooting) animation.Start();
            else animation.Stop();

            // Flip the gun
            float rotationDegree = MathHelper.ToDegrees(Rotation);
            if ((rotationDegree >= 90 && rotationDegree <= 180) || (rotationDegree <= -90 && rotationDegree >= -180))
            {
                spriteEffects = SpriteEffects.FlipVertically;
            }
            else
            {
                spriteEffects = SpriteEffects.None;
            }

            // Determine whether the gun need to render front to back of the holder sprite
            if (rotationDegree <= -15f && rotationDegree >= -160f)
            {
                RenderInfront = false;
            }
            else
            {
                RenderInfront = true;
            }

            // Gun postion and direction update
            position = holder.Position + offset; // Position will always follow the holder position
            Vector2 mousePos = Camera.ScreenToWorld(InputManager.MousePosition); // Convert mouse screen position to the world position
            Direction = mousePos - Position; // Rotation / Direction will always follow the mouse cursor
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
        }

        public void Shoot()
        {
            // If the animation is finish playing, then reset it
            if (shooting && animation.Finish)
            {
                shooting = false;
                animation.Reset(); // Reset the animation once its finish playing
            }

            if (InputManager.MouseLeftHold && !shooting)
            {
                if (InputManager.GetKeyDown(Keys.LeftShift)) // Player is walking, more accurate shooting
                {
                    accuracy = DEFAULT_ACCURACY * 0.2f; // NEED TO CHANGE LATER
                }
                else
                {
                    accuracy = DEFAULT_ACCURACY;
                }

                shooting = true; // Gun is shooting now
                GameManager.MainCamera.Shake();

                // Spawn Bullet
                Vector2 mousePos = Camera.ScreenToWorld(InputManager.MousePosition); // Convert mouse screen position to the world position
                Vector2 bulletDirection = mousePos - position;
                bulletDirection.Normalize();

                // Random the based on the accuracy of the player
                bulletDirection.X = Extensions.RandomRangeFloat(bulletDirection.X - accuracy, bulletDirection.X + accuracy);
                bulletDirection.Y = Extensions.RandomRangeFloat(bulletDirection.Y - accuracy, bulletDirection.Y + accuracy);

                // Fire Bullet
                DefaultBullet bullet = new DefaultBullet(position, bulletDirection, 150f, 900f);
                GameManager.Resources.FindSoundEffect("Gun_Shoot").Play();
                GameManager.AddGameObject(bullet); // Straight away add bullet to entity manager to run it immediately
            }
        }
    }
}
