using BulletManiac.AI;
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

            steerAgent = new SteeringAgent(this, 65f, 5f, true);
            steerAgent.SteeringBehavior = SteeringBehavior.Arrival;

            animationManager.AddAnimation(EnemyAction.Idle, new Animation(GameManager.Resources.FindTexture("Bat_Flying"), 7, 1, animationSpeed));
            animationManager.AddAnimation(EnemyAction.Move, new Animation(GameManager.Resources.FindTexture("Bat_Flying"), 7, 1, animationSpeed));
            animationManager.AddAnimation(EnemyAction.Hit, new Animation(GameManager.Resources.FindTexture("Bat_Hit"), 3, 1, 0.2f, looping: false));
            animationManager.AddAnimation(EnemyAction.Attack, new Animation(GameManager.Resources.FindTexture("Bat_Attack"), 10, 1, animationSpeed, looping: false));

            texture = animationManager.GetAnimation(currentAction).CurrentTexture; // Assign default texture to it (based on default behavior)
            origin = texture.Bounds.Center.ToVector2();
            scale = new Vector2(0.5f);

            // Shadow visual
            Texture2D shadowTexture = Extensions.CropTexture2D(GameManager.Resources.FindTexture("Shadow"), new Rectangle(0, 0, 64, 64)); // Crop a shadow texture
            shadowEffect = new TextureEffect(shadowTexture, this, shadowTexture.Bounds.Center.ToVector2(), new Vector2(0.25f), new Vector2(0f, 2f));
        }

        public override void Update(GameTime gameTime)
        {
            if(currentAction == EnemyAction.Move)
                steerAgent.Update(gameTime, GameManager.Player); // Bat is flying toward to the player

            // Texture flipping
            if(steerAgent.CurrentXDir == XDirection.Left)
                spriteEffects = SpriteEffects.FlipHorizontally;
            else
                spriteEffects = SpriteEffects.None;

            // When hit animation is finish playing
            if(currentAction == EnemyAction.Hit && animationManager.GetAnimation(EnemyAction.Hit).Finish)
            {
                currentAction = EnemyAction.Move;
                animationManager.GetAnimation(EnemyAction.Hit).Reset();
            }

            // Animation update
            animationManager.Update(currentAction, gameTime);
            texture = animationManager.CurrentAnimation.CurrentTexture;

            if (currentAction == EnemyAction.Move)
                Position += steerAgent.CurrentFinalVelocity * GameManager.DeltaTime;

            shadowEffect.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            shadowEffect.Draw(spriteBatch, gameTime); // Shadow always behind the player
            base.Draw(spriteBatch, gameTime);
        }

        protected override Rectangle CalculateBound()
        {
            if (texture == null) return Rectangle.Empty;
            if (spriteEffects == SpriteEffects.None)
            {
                Vector2 pos = position - (origin * scale / 1.1f) + new Vector2(2f, 0f); ;
                return new Rectangle((int)pos.X, (int)pos.Y + 3, (int)(texture.Width * scale.X / 1.25f), (int)(texture.Height * scale.Y / 1.1f));
            }
            else
            {
                Vector2 pos = position - (origin * scale / 1.1f) + new Vector2(2f, 0f);
                return new Rectangle((int)pos.X, (int)pos.Y + 3, (int)(texture.Width * scale.X / 1.25f), (int)(texture.Height * scale.Y / 1.1f));
            }
        }

        public override void Dispose()
        {
            steerAgent.Dispose();
            base.Dispose();
        }
    }
}
