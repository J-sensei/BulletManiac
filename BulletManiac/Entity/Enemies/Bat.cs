using BulletManiac.AI;
using BulletManiac.Managers;
using BulletManiac.Particle;
using BulletManiac.SpriteAnimation;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Entity.Enemies
{
    public class Bat : Enemy
    {
        private static SteeringSetting BAT_STERRING_SETTING = new SteeringSetting
        {
            DistanceToChase = 1000f,
            DistanceToFlee = 1000f
        };
        private static FlockSetting BAT_FLOCK_SETTING = new FlockSetting
        {
            Seperate = true,
            Alignment = true,
            Cohesion = false,
            NeighbourRadius = 65f
        };

        private const float BAT_SPEED = 65f;
        private const float BAT_ARRIVAL_RADIUS = 5f;
        private const int TOTAL_BAT_LEFT_TO_FLEE = 5;
        private SteeringAgent steerAgent;

        private float animationSpeed = 0.05f;
        public Bat(Vector2 position) : base(position)
        {
            animationManager = new AnimationManager();
            name = "Bat";
            hp = 30f;
            currentAction = EnemyAction.Move;

            steerAgent = new SteeringAgent(this, BAT_STERRING_SETTING, BAT_FLOCK_SETTING, BAT_SPEED, BAT_ARRIVAL_RADIUS, true);
            steerAgent.SteeringBehavior = SteeringBehavior.Arrival;

            animationManager.AddAnimation(EnemyAction.Idle, new Animation(ResourcesManager.FindTexture("Bat_Flying"), 7, 1, animationSpeed));
            animationManager.AddAnimation(EnemyAction.Move, new Animation(ResourcesManager.FindTexture("Bat_Flying"), 7, 1, animationSpeed));
            animationManager.AddAnimation(EnemyAction.Hit, new Animation(ResourcesManager.FindTexture("Bat_Hit"), 3, 1, 0.2f, looping: false));
            animationManager.AddAnimation(EnemyAction.Attack, new Animation(ResourcesManager.FindTexture("Bat_Attack"), 10, 1, animationSpeed, looping: false));

            //animationManager.AddAnimation(EnemyAction.Idle, new Animation(GameManager.Resources.FindTexture("FlyingEye_Flying"), 8, 1, animationSpeed));
            //animationManager.AddAnimation(EnemyAction.Move, new Animation(GameManager.Resources.FindTexture("FlyingEye_Flying"), 8, 1, animationSpeed));
            //animationManager.AddAnimation(EnemyAction.Hit, new Animation(GameManager.Resources.FindTexture("FlyingEye_Hit"), 4, 1, 0.2f, looping: false));
            //animationManager.AddAnimation(EnemyAction.Attack, new Animation(GameManager.Resources.FindTexture("FlyingEye_Attack"), 8, 1, animationSpeed, looping: false));
            //animationManager.AddAnimation(EnemyAction.Die, new Animation(GameManager.Resources.FindTexture("FlyingEye_Death"), 4, 1, animationSpeed, looping: false));

            origin = new Vector2(16f);
            //origin = new Vector2(75f);
            scale = new Vector2(0.5f);

            // Shadow visual
            shadowEffect = new TextureEffect(ResourcesManager.FindTexture("Shadow"),
                                new Rectangle(0, 0, 64, 64), // Crop the shadow sprite
                                this,
                                new Vector2(32f), new Vector2(0.25f), new Vector2(0f, 2f));
            deathSoundEffect = ResourcesManager.FindSoundEffect("Bat_Death");
        }

        public static void LoadContent()
        {
            ResourcesManager.LoadTexture("FlyingEye_Flying", "SpriteSheet/Enemy/Flying Eye/Flight");
            ResourcesManager.LoadTexture("FlyingEye_Death", "SpriteSheet/Enemy/Flying Eye/Death");
            ResourcesManager.LoadTexture("FlyingEye_Hit", "SpriteSheet/Enemy/Flying Eye/Take Hit");
            ResourcesManager.LoadTexture("FlyingEye_Attack", "SpriteSheet/Enemy/Flying Eye/Attack");
        }

        const float MOVE_CD = 0.25f;
        float moveCD = 0;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (currentAction == EnemyAction.Die) return;

            if (FlockManager.Find(Name).Count <= TOTAL_BAT_LEFT_TO_FLEE)
                steerAgent.SteeringBehavior = SteeringBehavior.Flee;
            else
                steerAgent.SteeringBehavior = SteeringBehavior.Arrival;

            if (currentAction == EnemyAction.Move)
            {
                // The steering behavior will update every MOVE_CD second
                moveCD -= Time.DeltaTime;
                if (moveCD <= 0f)
                {
                    steerAgent.Update(gameTime, GameManager.Player); // Bat is flying toward to the player
                    moveCD = MOVE_CD;
                }

                // Lock the bat inside the bound of the level
                Vector2 velocity = steerAgent.CurrentVelocity;
                if (Position.X < GameManager.CurrentLevel.Bound.X && velocity.X < 0f)
                    velocity.X = 0f;
                if (Position.X > GameManager.CurrentLevel.Bound.Width && velocity.X > 0f)
                    velocity.X = 0f;
                if (Position.Y < GameManager.CurrentLevel.Bound.Y && velocity.Y < 0f)
                    velocity.Y = 0f;
                if (Position.Y > GameManager.CurrentLevel.Bound.Height && velocity.Y > 0f)
                    velocity.Y = 0f;

                Position += velocity * Time.DeltaTime; // Update the position of the bat
            }

            // Texture flipping
            if (steerAgent.CurrentXDir == XDirection.Left)
                spriteEffects = SpriteEffects.FlipHorizontally;
            else
                spriteEffects = SpriteEffects.None;

            // When hit animation is finish playing (Recover from hit animation)
            if (currentAction == EnemyAction.Hit && animationManager.GetAnimation(EnemyAction.Hit).Finish)
            {
                currentAction = EnemyAction.Move;
                animationManager.GetAnimation(EnemyAction.Hit).Reset();
            }
        }

        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - origin * scale / 1.1f + new Vector2(2f, 0f);
            return new Rectangle((int)pos.X - 1, (int)pos.Y, (int)(origin.X * 2.2 * scale.X / 1.25f), (int)(origin.Y * 2.2 * scale.Y / 1.1f));
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void DeleteEvent()
        {
            steerAgent.Dispose();
            base.DeleteEvent();
        }
    }
}
