using BulletManiac.Collision;
using BulletManiac.Managers;
using BulletManiac.Particle;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private AnimationManager animationManager; // Manange the animation based on certain action
        protected EnemyAction currentAction = EnemyAction.Idle;

        public Enemy(Vector2 position)
        {
            name = "Enemy";
            this.position = position;
            animationManager = new AnimationManager();
            CollisionManager.Add(this, "Enemy"); // Add enemy into the collision manager
        }

        public override void CollisionEvent(GameObject other)
        {
            if(other.Name == "Bullet")
            {
                // Deal damaage to the enemy
                hp -= (other as Bullet.Bullet).Damage;

                GameManager.Resources.FindSoundEffect("Bullet_Hit").Play(); // Bullet Hit sound
                currentAction = EnemyAction.Hit; // Change player state
                Destroy(other); // Destroy bullet

                // If enemy hp is 0, destroy it
                if(hp <= 0)
                {
                    currentAction = EnemyAction.Die;
                    animationManager.Dispose();
                    Destroy(this);
                }
            }

            base.CollisionEvent(other);
        }

        public override void DeleteEvent()
        {
            TextureEffect effect = new TextureEffect(new Animation(GameManager.Resources.FindTexture("Destroy_Smoke"), 5, 1, 0.1f, looping: false),
                                                    Position, new Vector2(16, 16), new Vector2(1f), true);
            GameManager.AddGameObject(effect);
            base.DeleteEvent();
        }
    }
}
