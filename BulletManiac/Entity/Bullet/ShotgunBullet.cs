using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.Bullet
{
    public class ShotgunBullet : Bullet
    {
        const int BULLET_NUM = 5;
        List<DefaultBullet> bullets = new();

        public ShotgunBullet(Vector2 position, Vector2 direction, float speed = DEFAULT_SPEED, float initalSpeed = 0f) : base(position, direction, speed, initalSpeed)
        {
            for(int i = 0; i < BULLET_NUM; i++)
            {
                bullets.Add(new DefaultBullet(position, direction, speed, initalSpeed));
            }
            BulletUI = GameManager.Resources.FindTexture("Bullet_Fill");
            Direction = direction;
        }

        public override void UpdateShootPosition(Vector2 position, Vector2 direction, float speed = 100, float initalSpeed = 0)
        {
            float accuracy = 0.7f;
            base.UpdateShootPosition(position, direction, speed, initalSpeed);
            for (int i = 0; i < BULLET_NUM; i++)
            {
                Vector2 d = direction;
                d.X = Extensions.RandomRangeFloat(d.X - accuracy, d.X + accuracy);
                d.Y = Extensions.RandomRangeFloat(d.Y - accuracy, d.Y + accuracy);
                Console.WriteLine(d);
                bullets[i].UpdateShootPosition(position, direction, speed, initalSpeed);
            }
        }

        protected override Rectangle CalculateBound()
        {
            return Rectangle.Empty;
        }

        public override void Shoot()
        {
            float accuracy = 0.7f;
            //GameManager.AddGameObject(this);
            for (int i = 0; i < BULLET_NUM; i++)
            {
                Vector2 d = direction;
                d.X = Extensions.RandomRangeFloat(d.X - accuracy, d.X + accuracy);
                d.Y = Extensions.RandomRangeFloat(d.Y - accuracy, d.Y + accuracy);
                bullets[i].Direction = d;
                GameManager.AddGameObject(bullets[i]);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            for (int i = 0; i < BULLET_NUM; i++)
            {
                bullets[i].Update(gameTime);
            }

            if (bullets.Where(x => x.IsDestroyed).Count() == BULLET_NUM)
            {
                Console.WriteLine("Delete Shotgun Bullet");
                Destroy(this);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!IsDestroyed)
            {
                for (int i = 0; i < BULLET_NUM; i++)
                {
                    bullets[i].Draw(spriteBatch, gameTime);
                }
            }
        }
    }
}
