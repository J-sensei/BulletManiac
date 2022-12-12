using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.Player
{
    public class Gun : GameObject
    {
        private Animation animation;

        private float currentCooldown = 0f;
        private float cooldown = 0.12f;

        public Gun()
        {
            name = "Player Gun";
            scale = new Vector2(0.5f);
            origin = new Vector2(0f);
        }

        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - origin;
            return new Rectangle((int)pos.X, (int)pos.Y, texture.Width, texture.Height);
        }

        public override void Initialize()
        {
            GameManager.Resources.LoadTexture("Pistol", "SpriteSheet/Gun/Pistol");
            animation = new Animation(GameManager.Resources.FindTexture("Pistol"), 12, 1, cooldown / 12f);
            //animation.Stop();

            spriteEffects = SpriteEffects.None;
            base.Initialize();
        }

        public void Follow(GameObject gameObject, Vector2 offset)
        {
            position = gameObject.Position + offset;
        }

        public override void Update(GameTime gameTime)
        {
            animation.Update(gameTime);
            texture = animation.CurrentTexture;

            Direction = Vector2.Normalize(InputManager.MousePosition - Position);
            // Console.WriteLine(Rotation);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
        }
    }
}
