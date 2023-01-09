using BulletManiac.AI;
using BulletManiac.Managers;
using BulletManiac.Particle;
using BulletManiac.SpriteAnimation;
using BulletManiac.Tiled;
using BulletManiac.Tiled.AI;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;

namespace BulletManiac.Entity.Enemy
{
    public class Shadow : Enemy
    {
        private NavigationAgent navigationAgent;
        private float animationSpeed = 0.08f;

        private float pathfindCD = 2f;
        private float currentPathfindCD = 2f;

        public Shadow(Vector2 position) : base(position)
        {
            animationManager = new AnimationManager();
            name = "Shadow";
            hp = 80f;
            currentAction = EnemyAction.Idle;

            navigationAgent = new NavigationAgent(this);
            origin = new Vector2(32f);
            scale = new Vector2(0.5f);
        }

        public override void Initialize()
        {
            animationManager.AddAnimation(EnemyAction.Idle, new Animation(GameManager.Resources.FindTexture("Shadow_Idle"), 4, 1, 0.1f));
            animationManager.AddAnimation(EnemyAction.Move, new Animation(GameManager.Resources.FindTexture("Shadow_Move"), 8, 1, animationSpeed));
            animationManager.AddAnimation(EnemyAction.Hit, new Animation(GameManager.Resources.FindTexture("Shadow_Hit"), 3, 1, 0.1f, looping: false));
            animationManager.AddAnimation(EnemyAction.Attack, new Animation(GameManager.Resources.FindTexture("Shadow_Attack"), 6, 1, animationSpeed, looping: false));
            animationManager.AddAnimation(EnemyAction.Die, new Animation(GameManager.Resources.FindTexture("Shadow_Death"), 6, 1, animationSpeed, looping: false));

            deathSoundEffect = GameManager.Resources.FindSoundEffect("Shadow_Death");
            // Shadow Visual
            shadowEffect = new TextureEffect(GameManager.Resources.FindTexture("Shadow"),
                    new Rectangle(0, 0, 64, 64), // Crop the shadow sprite
                    this,
                    new Vector2(32f), new Vector2(0.5f), new Vector2(0f, -5f));

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {            
            base.Update(gameTime);
            if (currentAction == EnemyAction.Die) return; // Dont run anything else because enemy is dead

            // When hit animation is finish playing (Recover from hit animation)
            if (currentAction == EnemyAction.Hit && animationManager.GetAnimation(EnemyAction.Hit).Finish)
            {
                currentAction = EnemyAction.Move;
                animationManager.GetAnimation(EnemyAction.Hit).Reset();
            }

            if (navigationAgent.CurrentState == NavigationState.STOP)
            {
                if (currentAction != EnemyAction.Hit)
                    currentAction = EnemyAction.Idle; // Make enemy idle when its not in hit state
                currentPathfindCD -= Time.DeltaTime;
                if (currentPathfindCD <= 0f)
                {
                    navigationAgent.Pathfind(GameManager.CurrentLevel.TileGraph.RandomPosition); // Execute pathfinding
                    currentPathfindCD = pathfindCD;
                    currentAction = EnemyAction.Move;
                }
            }

            // Navigate the enemy when its moving state
            if (currentAction == EnemyAction.Move)
            {
                navigationAgent.Update(gameTime);
            }

            // Flip sprite
            if (navigationAgent.CurrentXDir == XDirection.Left)
                spriteEffects = SpriteEffects.None;
            else
                spriteEffects = SpriteEffects.FlipHorizontally;
        }

        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - (origin * scale / 2f) + new Vector2(2f, 0f);
            return new Rectangle((int)pos.X, (int)pos.Y + 3, (int)((origin.X * 2) * scale.X / 2.2f), (int)((origin.Y * 2) * scale.Y / 2.2f));
        }
    }
}
