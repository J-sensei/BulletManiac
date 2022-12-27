using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletManiac.Entity.Bullet;
using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace BulletManiac.Entity.Player
{
    public class Megazine
    {
        private int capacity; // Capacity of the bullet
        private readonly float bulletCD;
        private float currentBulletCD; // Time required for each bullet to put in megazine
        public bool Reloading { get; private set; } = false;
        public bool CanShoot
        {
            get
            {
                return (!Reloading);
            }
        }

        public int Capacity { get { return capacity; } }
        public Queue<BulletManiac.Entity.Bullet.Bullet> Bullets { get { return bullets; } }

        Queue<BulletManiac.Entity.Bullet.Bullet> bullets = new(); // current bullets in the megazine
        public Megazine(int capacity, float bulletCD)
        {
            this.capacity = capacity;
            this.bulletCD = bulletCD;
            currentBulletCD = bulletCD;

            for(int i = 0; i < capacity; i++)
            {
                bullets.Enqueue(new DefaultBullet(Vector2.Zero, Vector2.Zero));
            }
        }

        public void Update()
        {
            if (bullets.Count <= 0 || Reloading)
            {
                Reloading = true;
                currentBulletCD -= GameManager.DeltaTime;

                if (bullets.Count == capacity && currentBulletCD <= 0f)
                {
                    GameManager.Resources.FindSoundEffect("Pistol_Cock").Play();
                    currentBulletCD = bulletCD;
                    Reloading = false;
                }

                if (currentBulletCD <= 0f)
                {
                    bullets.Enqueue(new DefaultBullet(Vector2.Zero, Vector2.Zero));
                    currentBulletCD = bulletCD;
                    GameManager.Resources.FindSoundEffect("Mag_In").Play();
                }
            }
            else
            {
                Reloading = false;
            }
        }

        public Bullet.Bullet Shoot()
        {
            return bullets.Dequeue();
        }
    }
}
