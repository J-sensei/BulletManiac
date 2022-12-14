using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.Player
{
    public class Bullet : GameObject
    {
        private Animation animation;
        private float speed = 100f;

        public Bullet(Vector2 position, Vector2 direction, float speed = 100f)
        {
            this.name = "Bullet";
            this.position = position;
            Direction = direction;
            this.speed = speed;
        }

        public override void Initialize()
        {
            GameManager.Resources.LoadTexture("Test Bullet", "SpriteSheet/Bullet/Test_Bullets_16x16");
            Rectangle b = new Rectangle(0, 0, 16, 16);
            texture = Extensions.CropTexture2D(GameManager.Resources.FindTexture("Test Bullet"), b);
            origin = new Vector2(16f);
            base.Initialize();
        }

        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - origin;
            return new Rectangle((int)pos.X, (int)pos.Y, texture.Width, texture.Height);
        }

        public override void Update(GameTime gameTime)
        {
            position += Direction * speed * GameManager.DeltaTime; // Move the bullet
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
        }

        public override void CollisionEvent(GameObject other)
        {
            // Destroy the bullet
            // Destroy the enemy
            base.CollisionEvent(other);
        }
    }
}
