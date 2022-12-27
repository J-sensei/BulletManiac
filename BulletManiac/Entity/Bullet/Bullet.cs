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
        public Texture2D BulletUI { get; protected set; }
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

        public Bullet(Vector2 position, Vector2 direction, float speed = DEFAULT_SPEED, float initalSpeed = 0f)
        {
            name = "Bullet";
            this.position = position;
            Direction = direction;
            this.speed = speed;
            this.position += Direction * initalSpeed; // Move the bullet by the initial speed
            CollisionManager.Add(this, "Bullet");
        }

        public void UpdateShootPosition(Vector2 position, Vector2 direction, float speed = DEFAULT_SPEED, float initalSpeed = 0f)
        {
            this.position = position;
            Direction = direction;
            this.speed = speed;
            this.position += Direction * initalSpeed; // Move the bullet by the initial speed
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
                TextureEffect effect = new TextureEffect(new Animation(GameManager.Resources.FindTexture("Walking_Smoke"), 6, 1, 0.1f, looping: false),
                                                        Position, new Vector2(32, 32), true);
                GameManager.AddGameObject(effect);
                IsDestroyed = true; // Manually destroyvfor this
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
            base.CollisionEvent(other);
        }

        public override void DeleteEvent()
        {
            TextureEffect effect = new TextureEffect(new Animation(GameManager.Resources.FindTexture("Bullet1"), 5, 25, 0.05f, 3, false),
                        Position, new Vector2(8, 8), true);
            GameManager.AddGameObject(effect);
            base.DeleteEvent();
        }

        public override void Dispose()
        {
            if(animation != null)
            {
                animation.Dispose();
                animation = null;
            }
            base.Dispose();
        }
    }
}
