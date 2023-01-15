using BulletManiac.AI;
using BulletManiac.Managers;
using BulletManiac.SpriteAnimation;
using Microsoft.Xna.Framework;
using System;

namespace BulletManiac.Entity.Bullets
{
    public class TrackBullet : Bullet
    {
        private SteeringSetting setting = new SteeringSetting
        {
            DistanceToChase = 1000f,
            DistanceToFlee = 1000f
        };
        private SteeringAgent steeringAgent;
        /// <summary>
        /// The seek speed of the bullet
        /// </summary>
        private float traceSpeed;
        /// <summary>
        /// Follow target of the bullet
        /// </summary>
        private GameObject target;
        /// <summary>
        /// This bullet will only target one enemy after shoot
        /// </summary>
        private bool isTarget = false;
        /// <summary>
        /// Current velocity of the bullet
        /// </summary>
        private Vector2 velocity;

        public TrackBullet() : base(ResourcesManager.FindSoundEffect("Track_Shoot"))
        {
            Animation = new Animation(ResourcesManager.FindAnimation("TrackBullet_Animation"));
            Animation.Reset();

            basedDamage = 10f;
            origin = Animation.TextureBound / 2f; // Set the origin to the center of the texture
            scale = new Vector2(0.8f);
            BulletUI = ResourcesManager.FindTexture("Bullet_Track");
            traceSpeed = Speed * 1.2f;
            steeringAgent = new SteeringAgent(this, setting, FlockSetting.Default, traceSpeed);
            steeringAgent.SteeringBehavior = SteeringBehavior.Seek;
        }

        public override void Update(GameTime gameTime)
        {
            if (!isTarget)
            {
                target = GameManager.FindNearestEnemy(this);
                isTarget = true;
            }

            if (target != null && !target.IsDestroyed && (target.Position - position).Length() > 10f)
            {
                steeringAgent.Update(gameTime, target);
                velocity = steeringAgent.CurrentVelocity;
                Direction = SteeringAgent.GetHeading(velocity); // Update the Direction + rotation of the bullet
            }
            else // If bullet has no target or the distance is very close to the target
            {
                velocity = direction * Speed; // default velocity logic
            }

            base.Update(gameTime);
        }

        protected override Vector2 CalculateVelocity()
        {
            return velocity;
        }

        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - origin * scale / 1.2f;
            return new Rectangle((int)pos.X + 2, (int)pos.Y + 3, (int)(origin.X * 2 * scale.X / 1.5f), (int)(origin.Y * 2 * scale.Y / 1.5f));
        }
    }
}
