using BulletManiac.Managers;
using BulletManiac.Tiled.Pathfinding;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Entity.Enemy
{
    public class Spider : Enemy
    {
        private NavigationAgent agent;
        private AnimationManager animationManager;
        public Spider(Vector2 position) : base(position)
        {
            animationManager = new AnimationManager();
            name = "Spider";
            agent = new NavigationAgent(this);
            //texture = new Texture2D(GameManager.GraphicsDevice, 1, 1);
            //texture.SetData(new Color[] { Color.Blue });
            texture = Extensions.CropTexture2D(GameManager.Resources.FindTexture("Spider"), new Rectangle(0, 0, 32, 32));
            agent.OnMove += Move;
            origin = new Vector2(16, 16);
            scale = new Vector2(1f);
            spriteEffects = SpriteEffects.FlipHorizontally;
        }

        public override void Initialize()
        {
            agent.Initialize();
            base.Initialize();
        }

        void Move(Vector2 destination)
        {
            //Console.WriteLine(Position + " " + destination + " " + GameManager.Player.Position);
            Position += Vector2.Normalize(destination) * GameManager.DeltaTime;
            //Position += Vector2.Normalize(destination) * GameManager.DeltaTime * 100f;
        }

        public override void Update(GameTime gameTime)
        {
            agent.Update(gameTime, GameManager.Player.Position); // Always follow player
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
        }

        protected override Rectangle CalculateBound()
        {
            if (spriteEffects == SpriteEffects.None)
            {
                Vector2 pos = position - origin;
                return new Rectangle((int)pos.X + 8, (int)pos.Y + 24, (int)(texture.Width / 2f), (int)(texture.Height / 4f));
            }
            else
            {
                Vector2 pos = position - origin;
                return new Rectangle((int)pos.X + 8, (int)pos.Y + 24, (int)(texture.Width / 2f), (int)(texture.Height / 4f));
            }
        }
    }
}
