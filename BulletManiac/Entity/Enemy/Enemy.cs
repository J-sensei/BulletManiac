using BulletManiac.Collision;
using BulletManiac.Managers;
using BulletManiac.Particle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Entity.Enemy
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
            colorOverlay = GameManager.Resources.FindEffect("Color_Overlay");
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
                blinkTime -= GameManager.DeltaTime;
                if (blinkTime <= 0f)
                {
                    blinkTime = BLINK_TIME;
                    blink = false;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            shadowEffect.Draw(spriteBatch, gameTime); // Shadow always behind the player
            spriteBatch.End(); // End previous drawing session first inorder to start new one

            // Start new drawing session with shader (Everything between this draw call will be affect by the shader apply)
            if (blink)
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, transformMatrix: GameManager.MainCamera.Transform, effect: colorOverlay);
            else
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, transformMatrix: GameManager.MainCamera.Transform, effect: null);

            DrawAnimation(animationManager.CurrentAnimation, spriteBatch, gameTime);

            spriteBatch.End(); // End current drawing session
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, transformMatrix: GameManager.MainCamera.Transform); // Resume back to normal drawing session
        }

        public override void CollisionEvent(GameObject other)
        {
            base.CollisionEvent(other);
            if (hp <= 0) return; // If enemy is dead, no need to calculate the collision event anymore

            // Deal damage to the enemy
            if (other.Name == "Bullet")
            {
                // Deal damaage to the enemy
                hp -= (other as Bullet.Bullet).Damage;

                GameManager.Resources.FindSoundEffect("Bullet_Hit").Play(); // Bullet Hit sound
                currentAction = EnemyAction.Hit; // Change player state
                Destroy(other); // Destroy bullet
                blink = true;
            }

            if(other.Name == "Player")
            {
                Player.Player player = (other as Player.Player);
                player.TakeDamage(10f); // Test take damage
            }
        }

        public override void DeleteEvent()
        {
            TextureEffect effect = new TextureEffect(new Animation(GameManager.Resources.FindAnimation("Destroy_Smoke_Animation")),
                                                    Position, new Vector2(16, 16), new Vector2(1f), true);
            GameManager.AddGameObject(effect);
            base.DeleteEvent();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
