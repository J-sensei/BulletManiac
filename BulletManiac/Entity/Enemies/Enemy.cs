using BulletManiac.Collision;
using BulletManiac.Entity.Bullets;
using BulletManiac.Entity.Players;
using BulletManiac.Managers;
using BulletManiac.Particle;
using BulletManiac.SpriteAnimation;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Entity.Enemies
{
    /// <summary>
    /// Base class for enemy
    /// </summary>
    public abstract class Enemy : GameObject
    {
        /// <summary>
        /// Enemy actions, used to determine animation
        /// </summary>
        public enum EnemyAction
        {
            Idle, Move, Attack, Die, Hit
        }

        protected float hp = 100f;
        public float Damage { get { return basedDamage * DamageModifier; } }
        protected float basedDamage = 10f;
        public float DamageModifier { get; set; } = 1.0f;

        protected AnimationManager animationManager; // Manange the animation based on certain action
        protected TextureEffect shadowEffect; // Visual shadow effect
        protected EnemyAction currentAction = EnemyAction.Idle;
        protected SoundEffect deathSoundEffect;
        private bool hasDeathSoundPlayed = false;

        private bool blink = false; // true if get hit
        const float BLINK_TIME = 0.1f;
        private float blinkTime = BLINK_TIME;
        private Effect colorOverlay; // Shader for the blink effect
        public EnemyAction CurrentAction { get { return currentAction; } }

        public Enemy(Vector2 position)
        {
            name = "Enemy";
            this.position = position;
            animationManager = new AnimationManager();
            CollisionManager.Add(this, "Enemy"); // Add enemy into the collision manager
        }

        public override void Initialize()
        {
            // Shader initialize
            colorOverlay = ResourcesManager.FindEffect("Color_Overlay");
            colorOverlay.Parameters["overlayColor"].SetValue(Color.White.ToVector4());

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // Animation update
            animationManager.Update(currentAction, gameTime);

            // If enemy hp is 0, destroy it
            if (hp <= 0)
            {
                currentAction = EnemyAction.Die;
                if (!hasDeathSoundPlayed && deathSoundEffect != null)
                {
                    deathSoundEffect.Play();
                    hasDeathSoundPlayed = true;
                }

                if (animationManager.GetAnimation(EnemyAction.Die) != null)
                {
                    if (animationManager.GetAnimation(EnemyAction.Die).Finish) // Wait die animation finish and destroy it
                        Destroy(this);
                }
                else
                {
                    Destroy(this);
                }
            }

            if (blink)
            {
                blinkTime -= Time.DeltaTime;
                if (blinkTime <= 0f)
                {
                    blinkTime = BLINK_TIME;
                    blink = false;
                }
            }
            shadowEffect.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            shadowEffect.Draw(spriteBatch, gameTime); // Shadow always behind the player
            spriteBatch.End(); // End previous drawing session first inorder to start new one

            // Start new drawing session with shader (Everything between this draw call will be affect by the shader apply)
            if (blink)
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, transformMatrix: Camera.Main.Transform, effect: colorOverlay);
            else
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, transformMatrix: Camera.Main.Transform, effect: null);

            DrawAnimation(animationManager.CurrentAnimation, spriteBatch, gameTime);

            spriteBatch.End(); // End current drawing session
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, transformMatrix: Camera.Main.Transform); // Resume back to normal drawing session
        }

        public override void CollisionEvent(GameObject other)
        {
            base.CollisionEvent(other);
            if (hp <= 0) return; // If enemy is dead, no need to calculate the collision event anymore

            // Deal damage to the enemy
            if (other.Name == "Bullet")
            {
                // Deal damaage to the enemy
                hp -= (other as Bullet).Damage;

                ResourcesManager.FindSoundEffect("Bullet_Hit").Play(); // Bullet Hit sound
                currentAction = EnemyAction.Hit; // Change player state
                Destroy(other); // Destroy bullet
                blink = true;
            }

            if (other.Name == "Player")
            {
                Player player = other as Player;
                player.TakeDamage(Damage); // Test take damage
            }
        }

        float takeDamageCD = 0f;
        /// <summary>
        /// Take Damage every 1 second
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(float damage)
        {
            if (takeDamageCD <= 0f)
            {
                //Console.WriteLine(takeDamageCD + " " + (takeDamageCD <= 0f).ToString());
                hp -= damage;
                takeDamageCD = 0.05f; // 0.1 second cooldown
            }
            else
            {
                takeDamageCD -= Time.DeltaTime;
            }
        }

        protected Vector2 deathSmokeOffset = Vector2.Zero;
        public override void DeleteEvent()
        {
            TextureEffect effect = new TextureEffect(new Animation(ResourcesManager.FindAnimation("Destroy_Smoke_Animation")),
                                                    Position + deathSmokeOffset, new Vector2(16, 16), new Vector2(1.25f), true);
            GameManager.AddGameObject(effect);
            base.DeleteEvent();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
