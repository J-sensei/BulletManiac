using BulletManiac.Entity.Player;
using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.UI
{
    public class MagazineUI : GameObject
    {
        private readonly int capacity = 5;
        private readonly Gun gun;
        private readonly Magazine megazine;
        private readonly float angle = (float)Math.PI / 2.0f;  // 90 degrees

        public MagazineUI(Gun gun)
        {
            this.name = "Magazine UI";
            this.gun = gun;
            this.megazine = gun.Magazine;
            capacity = Gun.DEFAULT_BULLET;
            position = new Vector2(100f, 50f);
        }

        //public override void Update(GameTime gameTime)
        //{
            
        //}

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 offset = Vector2.Zero;

            // Loop each bullet and render the bullet UI
            foreach(Bullet.Bullet b in megazine.Bullets)
            {
                if(b.BulletUI == null) continue;
                spriteBatch.Draw(b.BulletUI, position + offset, null, Color.White, angle, Vector2.Zero, Vector2.One * 2, SpriteEffects.None, 0f);
                offset.Y += 20f;
            }
        }

        protected override Rectangle CalculateBound()
        {
            return Rectangle.Empty;
        }
    }
}
