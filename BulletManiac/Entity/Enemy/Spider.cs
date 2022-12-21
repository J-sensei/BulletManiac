using BulletManiac.Managers;
using BulletManiac.Tiled.Pathfinding;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Entity.Enemy
{
    public class Spider : Enemy
    {
        private NavigationAgent agent;
        public Spider(Vector2 position) : base(position)
        {
            name = "Spider";
            agent = new NavigationAgent(this);
            texture = new Texture2D(GameManager.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.Blue });
            agent.OnMove += Move;
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
            spriteBatch.Draw(texture, new Rectangle((int)Position.X, (int)Position.Y, 32, 32), Color.White); // Draw the debug red box
        }

        protected override Rectangle CalculateBound()
        {
            return Rectangle.Empty;
        }
    }
}
