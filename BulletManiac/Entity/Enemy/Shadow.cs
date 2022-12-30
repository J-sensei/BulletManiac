using BulletManiac.AI;
using BulletManiac.Managers;
using BulletManiac.Particle;
using BulletManiac.Tiled;
using BulletManiac.Tiled.AI;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.Enemy
{
    public class Shadow : Enemy
    {
        private NavigationAgent navigationAgent;
        private AnimationManager animationManager;
        private TextureEffect shadowEffect; // Visual shadow effect
        private float animationSpeed = 0.08f;

        private float pathfindCD = 2f;
        private float currentPathfindCD = 2f;

        private const int MAXIMUM_PATHFIND_PER_FRAME = 2;
        private static Queue<Shadow> pathfindQueue = new();

        public Shadow(Vector2 position) : base(position)
        {
            animationManager = new AnimationManager();
            name = "Shadow";
            hp = 80f;
            currentAction = EnemyAction.Move;

            navigationAgent = new NavigationAgent(this);

            animationManager.AddAnimation(EnemyAction.Idle, new Animation(GameManager.Resources.FindTexture("Shadow_Idle"), 4, 1, 0.1f));
            animationManager.AddAnimation(EnemyAction.Move, new Animation(GameManager.Resources.FindTexture("Shadow_Move"), 8, 1, animationSpeed));
            animationManager.AddAnimation(EnemyAction.Hit, new Animation(GameManager.Resources.FindTexture("Shadow_Hit"), 3, 1, 0.1f, looping: false));
            animationManager.AddAnimation(EnemyAction.Attack, new Animation(GameManager.Resources.FindTexture("Shadow_Attack"), 6, 1, animationSpeed, looping: false));
            animationManager.AddAnimation(EnemyAction.Die, new Animation(GameManager.Resources.FindTexture("Shadow_Death"), 6, 1, animationSpeed, looping: false));

            origin = new Vector2(32f);
            scale = new Vector2(0.5f);
            
            // Shadow Visual
            shadowEffect = new TextureEffect(GameManager.Resources.FindTexture("Shadow"),
                    new Rectangle(0, 0, 64, 64), // Crop the shadow sprite
                    this,
                    new Vector2(32f), new Vector2(0.5f), new Vector2(0f, -5f));
        }

        public void Pathfind(Shadow shadow)
        {
            // Get Random tile location
            var nodes = GameManager.CurrentLevel.TileGraph.Nodes;
            Vector2 pos = Tile.ToPosition(nodes.ElementAt(Extensions.Random.Next(nodes.Count)),
                                GameManager.CurrentLevel.Map.TileWidth,
                                GameManager.CurrentLevel.Map.TileHeight);


            shadow.navigationAgent.Pathfind(pos);
            shadow.currentPathfindCD = shadow.pathfindCD;
            shadow.currentAction = EnemyAction.Move;
        }

        //static int update = 0;
        //public static void GlobalUpdate()
        //{
        //    if (pathfindQueue.Count > 0 && update >= 42)
        //    {
        //        Console.WriteLine("A*" + " " + update);
        //        Pathfind(pathfindQueue.Dequeue());
        //        update = 0;
        //    }


        //    update++;
        //}
        Stopwatch stopwatch = new();
        public override void Update(GameTime gameTime)
        {
            // When hit animation is finish playing (Recover from hit animation)
            if (currentAction == EnemyAction.Hit && animationManager.GetAnimation(EnemyAction.Hit).Finish)
            {
                currentAction = EnemyAction.Move;
                animationManager.GetAnimation(EnemyAction.Hit).Reset();
            }

            if (navigationAgent.CurrentState == NavigationState.STOP)
            {
                currentAction = EnemyAction.Idle;
                currentPathfindCD -= GameManager.DeltaTime;
                if(currentPathfindCD <= 0f)
                {
                    // Add to queue
                    //if(!pathfindQueue.Contains(this))
                    //    pathfindQueue.Enqueue(this);
                    Pathfind(this);
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

            // Animation update
            animationManager.Update(currentAction, gameTime);

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
            Vector2 pos = position - (origin * scale / 2f) + new Vector2(2f, 0f);
            return new Rectangle((int)pos.X, (int)pos.Y + 3, (int)((origin.X * 2) * scale.X / 2.2f), (int)((origin.Y * 2) * scale.Y / 2.2f));
        }
    }
}
