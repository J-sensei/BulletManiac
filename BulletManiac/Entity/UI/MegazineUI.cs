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
    public class MegazineUI
    {
        private int capacity = 5;
        private readonly Gun gun;
        private readonly Megazine megazine;

        public MegazineUI(Gun gun)
        {
            this.gun = gun;
            this.megazine = gun.Megazine;
            capacity = Gun.DEFAULT_BULLET;
        }

        public void Update(GameTime gameTime)
        {
            
        }
        float angle = (float) Math.PI / 2.0f;  // 90 degrees
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Vector2 offset = Vector2.Zero;
            //for(int i = 0; i < megazine.Capacity; i++)
            //{
            //    spriteBatch.Draw()
            //}
            foreach(Bullet.Bullet b in megazine.Bullets)
            {
                if(b.BulletUI == null) continue;
                spriteBatch.Draw(b.BulletUI, new Vector2(100f, 50f) + offset, null, Color.White, angle, Vector2.Zero, Vector2.One * 2, SpriteEffects.None, 0f);
                offset.Y += 20f;
            }
        }
    }
}
