using BulletManiac.AI;
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
    // Idle => Moving => Attack => Idle (Repeat)
    public class Summoner : Enemy
    {
        const int MAXIMUM_BAT_SUMMON = 30;
        float animationSpeed = 0.1f;
        const float SUMMON_CD = 2f;
        float summonCD = SUMMON_CD;
        const float IDLE_CD = 2f;
        float idleCD = IDLE_CD;
        const float MOVING_CD = 2f;
        float movingCD = MOVING_CD;
        public Summoner(Vector2 position) : base(position)
        {
            animationManager = new AnimationManager();
            name = "Summoner";
            hp = 150f;
            currentAction = EnemyAction.Idle;
            scale = new Vector2(0.8f);
        }

        public override void Initialize()
        {
            const int WIDTH = 64;
            const int HEIGHT = 64;
            // Define the keys and animations
            animationManager.AddAnimation(EnemyAction.Idle, new Animation(GameManager.Resources.FindTexture("Summoner_SpriteSheet"), 8, WIDTH, HEIGHT, animationSpeed));
            animationManager.AddAnimation(EnemyAction.Move, new Animation(GameManager.Resources.FindTexture("Summoner_SpriteSheet"), 7, WIDTH, HEIGHT, animationSpeed, 3));
            animationManager.AddAnimation(EnemyAction.Die, new Animation(GameManager.Resources.FindTexture("Summoner_SpriteSheet"), 8, WIDTH, HEIGHT, animationSpeed, 9, looping: false));
            animationManager.AddAnimation(EnemyAction.Attack, new Animation(GameManager.Resources.FindTexture("Summoner_SpriteSheet"), 8, WIDTH, HEIGHT, animationSpeed, 5, looping: false));

            // Shadow Visual
            shadowEffect = new TextureEffect(GameManager.Resources.FindTexture("Shadow"),
                    new Rectangle(0, 0, 64, 64), // Crop the shadow sprite
                    this,
                    new Vector2(32f), new Vector2(0.6f), new Vector2(0f, 10f));
            origin = animationManager.CurrentAnimation.TextureBound / 2f;
            base.Initialize();
        }

        bool attacking = false;
        Vector2 summonOffset = new Vector2(6f, 8f);
        public override void Update(GameTime gameTime)
        {
            if (currentAction == EnemyAction.Hit)
            {
                if (!attacking)
                {
                    currentAction = EnemyAction.Idle;
                }
                else
                    currentAction = EnemyAction.Attack;
            }

            summonCD -= GameManager.DeltaTime;
            if(summonCD <= 0f && !attacking)
            {
                if(FlockManager.Find("Bat") == null || FlockManager.Find("Bat").Count <= MAXIMUM_BAT_SUMMON)
                {
                    currentAction = EnemyAction.Attack;
                    attacking = true;
                    summonCD = SUMMON_CD;
                }
            }

            if (currentAction == EnemyAction.Attack && animationManager.CurrentAnimation.Finish)
            {
                // Summon bat
                GameManager.AddGameObject(new Bat(Position + summonOffset));
                currentAction = EnemyAction.Idle;
                animationManager.GetAnimation(EnemyAction.Attack).Reset();
                attacking = false;

                // Smoke effect
                TextureEffect effect = new TextureEffect(new Animation(GameManager.Resources.FindAnimation("Destroy_Smoke_Animation")),
                                        Position + summonOffset, new Vector2(16, 16), new Vector2(1.5f), true);
                GameManager.AddGameObject(effect);
            }

            base.Update(gameTime);
        }

        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - origin;
            return new Rectangle((int)pos.X + 22, (int)pos.Y + 40, (int)((origin.X * 2) * scale.X / 3f), (int)((origin.Y * 2) * scale.Y / 3f));
        }
    }
}
