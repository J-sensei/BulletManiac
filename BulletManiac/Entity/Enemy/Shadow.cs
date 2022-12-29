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
        private float animationSpeed = 0.06f;

        private float pathfindCD = 2f;
        private float currentPathfindCD = 2f;

        public Shadow(Vector2 position) : base(position)
        {
            animationManager = new AnimationManager();
            name = "Shadow";
            hp = 80f;
            currentAction = EnemyAction.Move;

            navigationAgent = new NavigationAgent(this);

            animationManager.AddAnimation(EnemyAction.Idle, new Animation(GameManager.Resources.FindTexture("Shadow_Idle"), 4, 1, animationSpeed));
            animationManager.AddAnimation(EnemyAction.Move, new Animation(GameManager.Resources.FindTexture("Shadow_Move"), 8, 1, animationSpeed));
            animationManager.AddAnimation(EnemyAction.Hit, new Animation(GameManager.Resources.FindTexture("Shadow_Hit"), 3, 1, 0.1f, looping: false));
            animationManager.AddAnimation(EnemyAction.Attack, new Animation(GameManager.Resources.FindTexture("Shadow_Attack"), 6, 1, animationSpeed, looping: false));
            animationManager.AddAnimation(EnemyAction.Die, new Animation(GameManager.Resources.FindTexture("Shadow_Death"), 6, 1, animationSpeed, looping: false));

            texture = animationManager.GetAnimation(currentAction).CurrentTexture; // Assign default texture to it (based on default behavior)
            origin = texture.Bounds.Center.ToVector2();
            scale = new Vector2(0.5f);

            // Shadow visual
            Texture2D shadowTexture = Extensions.CropTexture2D(GameManager.Resources.FindTexture("Shadow"), new Rectangle(0, 0, 64, 64)); // Crop a shadow texture
            shadowEffect = new TextureEffect(shadowTexture, this, shadowTexture.Bounds.Center.ToVector2(), new Vector2(0.5f), new Vector2(0f, -5f));
        }

        public override void Update(GameTime gameTime)
        {
            // When hit animation is finish playing (Recover from hit animation)
            if (currentAction == EnemyAction.Hit && animationManager.GetAnimation(EnemyAction.Hit).Finish)
            {
                currentAction = EnemyAction.Move;
                animationManager.GetAnimation(EnemyAction.Hit).Reset();
            }

            if(navigationAgent.CurrentState == NavigationState.STOP)
            {
                currentAction = EnemyAction.Idle;
                currentPathfindCD -= GameManager.DeltaTime;
                if(currentPathfindCD <= 0f && (GameManager.Player.Position - Position).Length() < 100f)
                {
                    // Get Random tile location
                    var nodes = GameManager.CurrentLevel.TileGraph.Nodes;
                    Vector2 pos = Tile.ToPosition(nodes.ElementAt(Extensions.Random.Next(nodes.Count)), 
                                        GameManager.CurrentLevel.Map.TileWidth, 
                                        GameManager.CurrentLevel.Map.TileHeight);

                    navigationAgent.Pathfind(pos);
                    currentPathfindCD = pathfindCD;
                    currentAction = EnemyAction.Move;
                }
            }

            // Navigate the enemy when its moving state
            if (currentAction == EnemyAction.Move)
                navigationAgent.Update(gameTime);

            // Flip sprite
            if (navigationAgent.CurrentXDir == XDirection.Left)
                spriteEffects = SpriteEffects.None;
            else
                spriteEffects = SpriteEffects.FlipHorizontally;

            // Animation update
            animationManager.Update(currentAction, gameTime);
            texture = animationManager.CurrentAnimation.CurrentTexture;

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
            //if (spriteEffects == SpriteEffects.None)
            //{
            //    Vector2 pos = position - (origin * scale / 2f) + new Vector2(2f, 0f);
            //    return new Rectangle((int)pos.X, (int)pos.Y + 3, (int)(texture.Width * scale.X / 2.2f), (int)(texture.Height * scale.Y / 2.2f));
            //}
            //else
            //{
            //    Vector2 pos = position - (origin * scale / 2f) + new Vector2(2f, 0f);
            //    return new Rectangle((int)pos.X, (int)pos.Y + 3, (int)(texture.Width * scale.X / 2.2f), (int)(texture.Height * scale.Y / 2.2f));
            //}
            Vector2 pos = position - (origin * scale / 2f) + new Vector2(2f, 0f);
            return new Rectangle((int)pos.X, (int)pos.Y + 3, (int)(texture.Width * scale.X / 2.2f), (int)(texture.Height * scale.Y / 2.2f));
        }
    }
}
