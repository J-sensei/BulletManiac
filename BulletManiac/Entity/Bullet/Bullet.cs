using BulletManiac.Collision;
using BulletManiac.Managers;
using BulletManiac.Particle;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Entity.Bullet
{
    /// <summary>
    /// Generic bullet class
    /// </summary>
    public abstract class Bullet : GameObject
    {
        protected const float DEFAULT_SPEED = 100f;
        protected const float DEFAULT_BASED_DAMAGE = 10f;
        protected const float DEFAULT_DAMAGE_MULTIPLIER = 1f;
        protected const float DEFAULT_ANIMATION_SPEED = 0.2f;

        private Animation animation;
        protected float speed = 100f;

        /// <summary>
        /// Original damage of the bullet
        /// </summary>
        protected float basedDamage = DEFAULT_BASED_DAMAGE;
        /// <summary>
        /// Modify the base damage value
        /// </summary>
        protected float damageMultiplier = DEFAULT_DAMAGE_MULTIPLIER;
        /// <summary>
        /// How much damage deal to the enemy
        /// </summary>
        protected float damage;

        protected Animation Animation { get { return animation; } set { animation = value; } }

        public Bullet(Vector2 position, Vector2 direction, float speed = DEFAULT_SPEED)
        {
            name = "Bullet";
            this.position = position;
            Direction = direction;
            this.speed = speed;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // Update the animation
            if(animation != null)
            {
                animation.Update(gameTime);
                texture = animation.CurrentTexture;
            }

            // Destroy the bullet when it hit the wall
            if(CollisionManager.CheckTileCollision(this, Vector2.Zero))
            {
                // Add smoke effect when bullet is destroy
                AnimationEffect effect = new AnimationEffect(new Animation(GameManager.Resources.FindTexture("Walking_Smoke"), 6, 1, 0.1f, looping: false),
                                                        Position, new Vector2(32, 32), true);
                GameManager.AddGameObject(effect);
                Destroy(this);
            }

            // Default bahavior of the bullet, which is moving to the desired direction
            position += Direction * speed * GameManager.DeltaTime; // Move the bullet
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
        }

        public override void CollisionEvent(GameObject other)
        {
            // Destroy the bullet
            // Destroy the enemy
            base.CollisionEvent(other);
        }

        public override void Dispose()
        {
            animation.Dispose();
            animation = null;
            base.Dispose();
        }
    }
}
