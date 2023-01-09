using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletManiac.Entity.Bullet;
using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace BulletManiac.Entity.Player
{
    public class Magazine
    {
        private int capacity; // Capacity of the bullet
        private readonly float bulletCD;
        private float currentBulletCD; // Time required for each bullet to put in megazine
        public bool Reloading { get; private set; } = false;
        public bool CanShoot
        {
            get
            {
                return bullets.Count > 0;
            }
        }

        public int Capacity { get { return capacity; } }
        public Queue<BulletManiac.Entity.Bullet.Bullet> Bullets { get { return bullets; } }

        Queue<BulletManiac.Entity.Bullet.Bullet> bullets = new(); // current bullets in the megazine
        public Magazine(int capacity, float bulletCD)
        {
            this.capacity = capacity;
            this.bulletCD = bulletCD;
            currentBulletCD = bulletCD;

            // Add the initial bullets
            for(int i = 0; i < capacity; i++)
                bullets.Enqueue(LoadBullet());
        }

        public void Update(bool shooting)
        {
            //if (bullets.Count < capacity && !shooting)
            //{
            //    Reloading = true;
            //    currentBulletCD -= GameManager.DeltaTime;

            //    //if (bullets.Count == capacity)
            //    //{
            //    //    GameManager.Resources.FindSoundEffect("Pistol_Cock").Play();
            //    //    currentBulletCD = bulletCD;
            //    //    Reloading = false;
            //    //}

            //    if (currentBulletCD <= 0f)
            //    {
            //        bullets.Enqueue(new DefaultBullet(Vector2.Zero, Vector2.Zero));
            //        currentBulletCD = bulletCD;
            //        GameManager.Resources.FindSoundEffect("Mag_In").Play();
            //        if (bullets.Count == capacity)
            //        {
            //            //GameManager.Resources.FindSoundEffect("Pistol_Cock").Play();
            //            Reloading = false;
            //        }
            //    }
            //}
            //else
            //{
            //    Reloading = false;
            //}

            if (bullets.Count == 0)
            {
                if(Reloading == false) GameManager.Resources.FindSoundEffect("Mag_In").Play(); // When bullet is empty, play a sound
                Reloading = true;
                currentBulletCD -= GameManager.DeltaTime;

                if (currentBulletCD <= 0f)
                {
                    // Add the bullets
                    for (int i = 0; i < capacity; i++)
                        bullets.Enqueue(LoadBullet());

                    currentBulletCD = bulletCD; // Reset CD
                    GameManager.Resources.FindSoundEffect("Pistol_Cock").Play(); // Play a sound when reload is finish
                }
            }
            else
            {
                Reloading = false;
            }
        }

        Bullet.Bullet LoadBullet()
        {
            return new ExplosionBullet();
            int rand = Extensions.Random.Next(3);
            Console.WriteLine(rand);
            switch (rand)
            {
                case 0:
                    return new DefaultBullet();
                case 1:
                    return new TrackBullet();
                case 2:
                    return new ShotgunBullet();
                default:
                    throw new Exception("Random bullet reload is out of bound");
            }
        }

        public Bullet.Bullet Shoot()
        {
            return bullets.Dequeue();
        }
    }
}
