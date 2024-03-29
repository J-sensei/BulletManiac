﻿using BulletManiac.Collision;
using BulletManiac.Managers;
using BulletManiac.Particle;
using BulletManiac.SpriteAnimation;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Entity.Bullets
{
    /// <summary>
    /// Generic bullet class
    /// </summary>
    public abstract class Bullet : GameObject
    {
        #region Constants
        protected const float DEFAULT_SPEED = 100f;
        protected const float DEFAULT_BASED_DAMAGE = 10f;
        protected const float DEFAULT_DAMAGE_MULTIPLIER = 1f;
        protected const float DEFAULT_ANIMATION_SPEED = 0.2f;
        #endregion

        public static float SpeedModifier = 1.0f;

        public SoundEffect ShootingSound { get; private set; }
        private Animation animation; // Animation of the bullet (All bullet should use animation to draw)
        /// <summary>
        /// Bullet Texture to render in the UI to indicate player which bullet is in the megazine
        /// </summary>
        public Texture2D BulletUI { get; protected set; }
        /// <summary>
        /// Current speed of the bullet
        /// </summary>
        protected float speed = DEFAULT_SPEED;
        protected float Speed { get { return speed * SpeedModifier; } }

        /// <summary>
        /// Original damage of the bullet
        /// </summary>
        protected float basedDamage = DEFAULT_BASED_DAMAGE;
        /// <summary>
        /// Modify the base damage value
        /// </summary>
        public static float DamageMultiplier = DEFAULT_DAMAGE_MULTIPLIER;
        /// <summary>
        /// How much damage deal to the enemy
        /// </summary>
        public float Damage
        {
            get { return basedDamage * DamageMultiplier; }
        }

        /// <summary>
        /// Animation of the bullet (All bullet should use animation to draw)
        /// </summary>
        protected Animation Animation { get { return animation; } set { animation = value; } }

        public Bullet(Vector2 position, Vector2 direction, float speed = DEFAULT_SPEED, float initalSpeed = 0f) : base("Bullet")
        {
            this.position = position;
            Direction = direction;
            this.speed = speed;
            this.position += Direction * initalSpeed; // Move the bullet by the initial speed

            CollisionManager.Add(this, "Bullet");
        }

        public Bullet(SoundEffect soundEffect) : base("Bullet")
        {
            ShootingSound = soundEffect;
        }

        public virtual void Shoot(Vector2 position, Vector2 direction, float speed = DEFAULT_SPEED, float initalSpeed = 0f)
        {
            this.position = position;
            Direction = direction;
            this.speed = speed;
            this.position += Direction * initalSpeed; // Move the bullet by the initial speed
            CollisionManager.Add(this, "Bullet");
            GameManager.AddGameObject(this);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // Update the animation
            if (animation != null)
            {
                animation.Update(gameTime);
            }

            // Destroy the bullet when it hit the wall
            if (CollisionManager.CheckTileCollision(this, Vector2.Zero))
            {
                DeleteEventWall();
            }

            // Default bahavior of the bullet, which is moving to the desired direction
            position += CalculateVelocity() * Time.DeltaTime; // Move the bullet
            base.Update(gameTime);
        }

        /// <summary>
        /// Bullet velocity calculation logic
        /// </summary>
        /// <returns></returns>
        protected virtual Vector2 CalculateVelocity()
        {
            return Direction * Speed;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawAnimation(animation, spriteBatch, gameTime);
        }

        public override void CollisionEvent(ICollidable other)
        {
            base.CollisionEvent(other);
        }

        public override void DeleteEvent()
        {
            TextureEffect effect = new TextureEffect(new Animation(ResourcesManager.FindTexture("Bullet_Destroy"), 8, 1, 0.06f, 1, false),
                        Position, new Vector2(16f), new Vector2(0.45f), true);
            GameManager.AddGameObject(effect);
            base.DeleteEvent();
        }

        /// <summary>
        /// Delete event for bullet when colliding with the wall
        /// </summary>
        protected virtual void DeleteEventWall()
        {
            // Add smoke effect when bullet is destroy
            TextureEffect effect = new TextureEffect(new Animation(ResourcesManager.FindAnimation("Destroy_Smoke_Animation")),
                                    Position, new Vector2(16, 16), new Vector2(0.65f), true);
            GameManager.AddGameObject(effect);
            IsDestroyed = true; // Destroy the bullet without trigger the delete event (as delete event is using when collide with enemy)
        }

        public override void Dispose()
        {
            if (animation != null)
            {
                animation.Dispose();
                animation = null;
            }
            base.Dispose();
        }
    }
}
