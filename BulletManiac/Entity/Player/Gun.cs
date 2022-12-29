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

        public const int DEFAULT_BULLET = 5;
        public int CurrentBullet { get; private set; } = 5;
        const float DEFAULT_RELOAD_CD = 0.25f;

        const float MIN_SHOOT_ANIMATION_SPEED = 0.01f;
        const float MAX_SHOOT_ANIMATION_SPEED = 0.05f;
        float currentShootAnimationSpeed = MIN_SHOOT_ANIMATION_SPEED; // Test

        const float DEFAULT_ACCURACY = 0.25f;
        float accuracy = DEFAULT_ACCURACY; // The lower the number, the better accuracy

        /// <summary>
        /// True need to render infront of the holder
        /// </summary>
        public bool RenderInfront { get; private set; }

        public Magazine Magazine { get; private set; }
        public bool Reloading { get { return Magazine.Reloading; } }

        public Gun(GameObject holder)
        {
            name = "Player Gun";
            scale = new Vector2(0.4f);
            this.holder = holder;

            animation = new Animation(GameManager.Resources.FindTexture("Player_Pistol"), 12, 1, 0.001f, looping: false);
            animation.Stop();
            origin = new Vector2(0f, 16f);
            Magazine = new Magazine(DEFAULT_BULLET, DEFAULT_RELOAD_CD);
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

            // If the gun is shooting, the animation need to play
            if (shooting) animation.Start();
            else animation.Stop();

            // Gun postion and direction update
            position = holder.Position + offset; // Position will always follow the holder position
            Vector2 mousePos = Camera.ScreenToWorld(InputManager.MousePosition); // Convert mouse screen position to the world position
            Direction = mousePos - Position; // Rotation / Direction will always follow the mouse cursor

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

            Shoot(); // Gun shooting
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawAnimation(animation, spriteBatch, gameTime);
        }

        public void Shoot()
        {
            Magazine.Update(shooting); // Reloading logic
            // If the animation is finish playing, then reset it or the gun is not shooting
            if (shooting && animation.Finish || (!shooting && animation.CurrentFrameIndex > 0))
            {
                shooting = false;
                animation.Reset(); // Reset the animation once its finish playing
            }

            // If gun is not shooting and player want to trigger it
            if (InputManager.MouseLeftHold && !shooting && Magazine.CanShoot)
            {
                // Player is walking, more accurate shooting
                if (InputManager.GetKeyDown(Keys.LeftShift))
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
                //DefaultBullet bullet = new DefaultBullet(position, bulletDirection, 150f, 16f);
                Bullet.Bullet bullet = Magazine.Shoot(); // Get the current bullet from the megazine
                bullet.UpdateShootPosition(position, bulletDirection, 150f, 16f);
                GameManager.Resources.FindSoundEffect("Gun_Shoot").Play();
                GameManager.AddGameObject(bullet); // Straight away add bullet to entity manager to run it immediately
            }
        }
    }
}
