using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BulletManiac.Entity.Bullets
{
    public class ShotgunBullet : Bullet
    {
        const int BULLET_NUM = 5;
        List<DefaultBullet> bullets = new();

        public ShotgunBullet() : base(ResourcesManager.FindSoundEffect("Shotgun_Shoot"))
        {
            for (int i = 0; i < BULLET_NUM; i++)
            {
                bullets.Add(new DefaultBullet());
            }
            BulletUI = ResourcesManager.FindTexture("Bullet_Shotgun");
            Direction = direction;
        }

        public override void Shoot(Vector2 position, Vector2 direction, float speed = 100, float initalSpeed = 0)
        {
            float accuracy = 0.7f;
            // Update the shoot of each shoot gun bullet shells
            for (int i = 0; i < BULLET_NUM; i++)
            {
                Vector2 d = direction;
                d.X = Extensions.RandomRangeFloat(d.X - accuracy, d.X + accuracy);
                d.Y = Extensions.RandomRangeFloat(d.Y - accuracy, d.Y + accuracy);

                bullets[i].Shoot(position, direction, speed, initalSpeed);
                bullets[i].Direction = d;
            }

            Destroy(this); // Destroy the shotgun bullet after all shotgun sheel is fired
        }

        protected override Rectangle CalculateBound()
        {
            return Rectangle.Empty;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // No update method for the shotgun itself
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            // No draw method for the shotgun bullet itself
        }

        public override void DeleteEvent()
        {
            // Overridde and empty the parent delete event for bullet
        }
    }
}
