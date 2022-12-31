using BulletManiac.AI;
using BulletManiac.Collision;
using BulletManiac.Managers;
using BulletManiac.Particle;
using BulletManiac.Tiled;
using BulletManiac.Tiled.AI;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        const float MIN_TILE_DISTANCE_FROM_PLAYER = 200f;
        private static SteeringSetting SUMMONER_STERRING_SETTING = new SteeringSetting
        {
            DistanceToChase = 1000f,
            DistanceToFlee = 1000f
        };
        private SteeringAgent steerAgent;
        private NavigationAgent navigationAgent;

        float speed = 70f;
        const int MAXIMUM_BAT_SUMMON = 30;
        float animationSpeed = 0.1f;
        const float SUMMON_CD = 2f;
        float summonCD = SUMMON_CD;
        const float CHANGE_STATE_CD = 4f;
        float changeStateCD = CHANGE_STATE_CD;
        public Summoner(Vector2 position) : base(position)
        {
            animationManager = new AnimationManager();
            name = "Summoner";
            hp = 150f;
            currentAction = EnemyAction.Idle;
            scale = new Vector2(0.8f);

            steerAgent = new SteeringAgent(this, SUMMONER_STERRING_SETTING, FlockSetting.Default, speed);
            steerAgent.SteeringBehavior = SteeringBehavior.Flee;

            navigationAgent = new NavigationAgent(this, speed);
            deathSmokeOffset = new Vector2(0f, 22f);
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
                    currentAction = state;
                }
                else
                    currentAction = EnemyAction.Attack;
            }

            changeStateCD -= GameManager.DeltaTime;
            if(changeStateCD <= 0f)
            {
                // Change State
                StateLogic();
                changeStateCD = CHANGE_STATE_CD;
            }

            if(currentAction == EnemyAction.Attack)
                Summon();

            if(currentAction == EnemyAction.Move)
            {
                //Vector2 velocity = steerAgent.Update(gameTime, GameManager.Player);
                //steerAgent.Update(gameTime, GameManager.Player);

                //Vector2 velocity = steerAgent.CurrentFinalVelocity * GameManager.DeltaTime;
                //Vector2 v = velocity;

                //bool colX, colY;

                //if (velocity.X >= 0)
                //    colX = CollisionManager.CheckTileCollision(this, new Vector2(velocity.X, 0) * 1.5f);
                //else
                //    colX = CollisionManager.CheckTileCollision(this, new Vector2(velocity.X, 0) * 1f);

                //if (velocity.Y >= 0)
                //    colY = CollisionManager.CheckTileCollision(this, new Vector2(0, velocity.Y) * 1.5f);
                //else
                //    colY = CollisionManager.CheckTileCollision(this, new Vector2(0, velocity.Y) * 1f);

                //if (colX && colY)
                //{
                //    v.X = 0f;
                //    v.Y = 0f;
                //}
                //else
                //{
                //    if (colX)
                //    {
                //        v.X = 0f;
                //        v.Y = (velocity.Y >= 0f ? speed : -speed) * GameManager.DeltaTime;
                //    }

                //    if (colY)
                //    {
                //        v.X = (velocity.X > 0f ? speed : -speed) * GameManager.DeltaTime;
                //        v.Y = 0f;
                //    }
                //}

                //Console.WriteLine(steerAgent.CurrentFinalVelocity + " " + velocity + " " + v + " Col:(" + colX.ToString() +","+ colY.ToString() + ")");
                //Position += v;
                //if(steerAgent.CurrentXDir == XDirection.Left)
                //{
                //    spriteEffects = SpriteEffects.FlipHorizontally;
                //}
                //else
                //{
                //    spriteEffects = SpriteEffects.None;
                //}

                // Find random path that is x distance more than the player
                if(navigationAgent.CurrentState == NavigationState.STOP)
                {
                    // Get target nodes
                    HashSet<Tile> targetNodes = GameManager.CurrentLevel.TileGraph.Nodes.Where(x => (GameManager.Player.Position - Tile.ToPosition(x, GameManager.CurrentLevel.Map.TileWidth,
                            GameManager.CurrentLevel.Map.TileHeight)).Length() > MIN_TILE_DISTANCE_FROM_PLAYER).ToHashSet();
                    if(targetNodes.Count() > 0)
                    {
                        Vector2 pos = Tile.ToPosition(targetNodes.ElementAt(Extensions.Random.Next(targetNodes.Count)),
                            GameManager.CurrentLevel.Map.TileWidth,
                            GameManager.CurrentLevel.Map.TileHeight);
                        navigationAgent.Pathfind(pos); // Execute pathfinding
                    }
                }
                else
                {
                    navigationAgent.Update(gameTime);
                }
            }

            base.Update(gameTime);
        }

        private void Summon()
        {
            if (!attacking)
            {
                if (FlockManager.Find("Bat") == null || FlockManager.Find("Bat").Count <= MAXIMUM_BAT_SUMMON)
                {
                    currentAction = EnemyAction.Attack;
                    attacking = true;
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
        }

        EnemyAction state;
        void StateLogic()
        {
            switch (currentAction)
            {
                case EnemyAction.Idle:
                    navigationAgent.Reset();
                    state = EnemyAction.Move;
                    break;
                case EnemyAction.Attack:
                    state = EnemyAction.Idle;
                    break;
                case EnemyAction.Move:
                    state = EnemyAction.Attack;
                    break;
                default:
                    state = EnemyAction.Idle;
                    break;
            }
            currentAction = state;
        }

        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - origin;
            return new Rectangle((int)pos.X + 22, (int)pos.Y + 40, (int)((origin.X * 2) * scale.X / 3f), (int)((origin.Y * 2) * scale.Y / 3f));
        }
    }
}
