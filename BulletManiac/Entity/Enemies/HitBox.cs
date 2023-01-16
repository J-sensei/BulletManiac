using BulletManiac.Collision;
using BulletManiac.Entity.Players;
using BulletManiac.Managers;
using BulletManiac.SpriteAnimation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.Enemies
{
    /// <summary>
    /// Hit box that will deal damage when colliding to the player
    /// </summary>
    public class HitBox : GameObject
    {
        private readonly float damage;
        private readonly Animation animation;
        private readonly List<int> frameToDrawBound;

        private readonly bool enableEnemyDamage = false;
        private readonly bool enablePlayerDamage = false;
        private SoundEffect soundEffect;
        private bool soundEffectPlay = false;
        private int frameToPlay;

        public HitBox(Animation animation, Vector2 position, Vector2 scale, List<int> frameToDrawBound, float damage = 50f, bool enableEnemyDamage = false, bool enablePlayerDamage = false)
        {
            name = "Hit Box";
            this.position = position;
            this.animation = animation;
            origin = animation.TextureBound / 2f;
            this.frameToDrawBound = frameToDrawBound;
            this.damage = damage;
            CollisionManager.Add(this, "Hit Box");
            this.enableEnemyDamage = enableEnemyDamage;
            this.enablePlayerDamage = enablePlayerDamage;
        }

        /// <summary>
        /// Add sound effect to the hitbox to play at certain frame
        /// </summary>
        /// <param name="soundEffect"></param>
        /// <param name="frameToPlay"></param>
        public void AddSoundEffect(SoundEffect soundEffect, int frameToPlay = 0)
        {
            this.soundEffect = soundEffect;
            this.frameToPlay = frameToPlay;
        }

        public override void Update(GameTime gameTime)
        {
            animation.Update(gameTime);
            if (animation.CurrentFrameIndex == frameToPlay && !soundEffectPlay)
            {
                if (soundEffect != null)
                {
                    //soundEffect.Play();
                    AudioManager.Play(soundEffect);
                }

                soundEffectPlay = true;
            }

            if (animation.Finish)
            {
                Destroy(this);
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawAnimation(animation, spriteBatch, gameTime);
        }

        protected override Rectangle CalculateBound()
        {
            if (frameToDrawBound.Contains(animation.CurrentFrameIndex))
            {
                Vector2 pos = position - origin;
                return new Rectangle((int)pos.X, (int)pos.Y, (int)animation.TextureBound.X, (int)animation.TextureBound.Y);
            }
            else
            {
                return Rectangle.Empty;
            }
        }

        List<Enemy> takenDamage = new();
        public override void CollisionEvent(ICollidable other)
        {
            if (other.Tag == "Player" && enablePlayerDamage)
            {
                Player player = other.GameObject as Player;
                player.TakeDamage(damage); // Test take damage
            }

            if (other.Tag == "Enemy" && enableEnemyDamage)
            {
                Enemy enemy = other.GameObject as Enemy;
                if (!takenDamage.Contains(enemy)) // do not damage some enemy again
                {
                    enemy.TakeDamage(damage);
                    takenDamage.Add(enemy);
                }
            }

            base.CollisionEvent(other);
        }

        public override void Dispose()
        {
            takenDamage.Clear();
            base.Dispose();
        }
    }
}
