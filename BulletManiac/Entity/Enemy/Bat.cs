﻿using BulletManiac.AI;
using BulletManiac.Managers;
using BulletManiac.Particle;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Entity.Enemy
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
        private AnimationManager animationManager;
        private TextureEffect shadowEffect; // Visual shadow effect

        private float animationSpeed = 0.05f;
        public Bat(Vector2 position) : base(position)
        {
            animationManager = new AnimationManager();
            name = "Bat";
            hp = 30f;
            currentAction = EnemyAction.Move;

            steerAgent = new SteeringAgent(this, BAT_STERRING_SETTING, BAT_FLOCK_SETTING, BAT_SPEED, BAT_ARRIVAL_RADIUS, true);
            steerAgent.SteeringBehavior = SteeringBehavior.Arrival;

            animationManager.AddAnimation(EnemyAction.Idle, new Animation(GameManager.Resources.FindTexture("Bat_Flying"), 7, 1, animationSpeed));
            animationManager.AddAnimation(EnemyAction.Move, new Animation(GameManager.Resources.FindTexture("Bat_Flying"), 7, 1, animationSpeed));
            animationManager.AddAnimation(EnemyAction.Hit, new Animation(GameManager.Resources.FindTexture("Bat_Hit"), 3, 1, 0.2f, looping: false));
            animationManager.AddAnimation(EnemyAction.Attack, new Animation(GameManager.Resources.FindTexture("Bat_Attack"), 10, 1, animationSpeed, looping: false));

            origin = new Vector2(16f);
            scale = new Vector2(0.5f);

            // Shadow visual
            shadowEffect = new TextureEffect(GameManager.Resources.FindTexture("Shadow"),
                                new Rectangle(0, 0, 64, 64), // Crop the shadow sprite
                                this,
                                new Vector2(32f), new Vector2(0.25f), new Vector2(0f, 2f));
        }

        public override void Update(GameTime gameTime)
        {
            if (FlockManager.Find(Name).Count <= TOTAL_BAT_LEFT_TO_FLEE)
            {
                steerAgent.SteeringBehavior = SteeringBehavior.Flee;
            }
            else
            {
                steerAgent.SteeringBehavior = SteeringBehavior.Arrival;
            }

            if (currentAction == EnemyAction.Move)
                steerAgent.Update(gameTime, GameManager.Player); // Bat is flying toward to the player

            // Texture flipping
            if(steerAgent.CurrentXDir == XDirection.Left)
                spriteEffects = SpriteEffects.FlipHorizontally;
            else
                spriteEffects = SpriteEffects.None;

            // When hit animation is finish playing (Recover from hit animation)
            if(currentAction == EnemyAction.Hit && animationManager.GetAnimation(EnemyAction.Hit).Finish)
            {
                currentAction = EnemyAction.Move;
                animationManager.GetAnimation(EnemyAction.Hit).Reset();
            }

            // Animation update
            animationManager.Update(currentAction, gameTime);
            
            if (currentAction == EnemyAction.Move)
                Position += steerAgent.CurrentFinalVelocity * GameManager.DeltaTime;

            shadowEffect.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            shadowEffect.Draw(spriteBatch, gameTime); // Shadow always behind the player
            DrawAnimation(animationManager.CurrentAnimation, spriteBatch, gameTime);
        }

        protected override Rectangle CalculateBound()
        {
            //if (spriteEffects == SpriteEffects.None)
            //{
            //    Vector2 pos = position - (origin * scale / 1.1f) + new Vector2(2f, 0f); ;
            //    return new Rectangle((int)pos.X, (int)pos.Y + 3, (int)(texture.Width * scale.X / 1.25f), (int)(texture.Height * scale.Y / 1.1f));
            //}
            //else
            //{
            //    Vector2 pos = position - (origin * scale / 1.1f) + new Vector2(2f, 0f);
            //    return new Rectangle((int)pos.X, (int)pos.Y + 3, (int)(texture.Width * scale.X / 1.25f), (int)(texture.Height * scale.Y / 1.1f));
            //}
            Vector2 pos = position - (origin * scale / 1.1f) + new Vector2(2f, 0f);
            return new Rectangle((int)pos.X, (int)pos.Y + 3, (int)((origin.X * 2) * scale.X / 1.25f), (int)((origin.Y * 2) * scale.Y / 1.1f));
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void DeleteEvent()
        {
            GameManager.Resources.FindSoundEffect("Bat_Death").Play();
            steerAgent.Dispose();
            base.DeleteEvent();
        }
    }
}
