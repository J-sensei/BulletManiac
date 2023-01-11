using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletManiac.Entity.Bullets;
using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace BulletManiac.Entity.Players
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
        public Queue<Bullet> Bullets { get { return bullets; } }

        Queue<Bullet> bullets = new(); // current bullets in the megazine
        public Magazine(int capacity, float bulletCD)
        {
            this.capacity = capacity;
            this.bulletCD = bulletCD;
            currentBulletCD = bulletCD;

            // Add the initial bullets
            for (int i = 0; i < capacity; i++)
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
                if (Reloading == false) ResourcesManager.FindSoundEffect("Mag_In").Play(); // When bullet is empty, play a sound
                Reloading = true;
                currentBulletCD -= Time.DeltaTime;

                if (currentBulletCD <= 0f)
                {
                    // Add the bullets
                    for (int i = 0; i < capacity; i++)
                        bullets.Enqueue(LoadBullet());

                    currentBulletCD = bulletCD; // Reset CD
                    ResourcesManager.FindSoundEffect("Pistol_Cock").Play(); // Play a sound when reload is finish
                }
            }
            else
            {
                Reloading = false;
            }
        }

        Bullet LoadBullet()
        {
            int rand = Extensions.Random.Next(4);
            switch (rand)
            {
                case 0:
                    return new DefaultBullet();
                case 1:
                    return new TrackBullet();
                case 2:
                    return new ShotgunBullet();
                case 3:
                    return new ExplosionBullet();
                default:
                    throw new Exception("Random bullet reload is out of bound");
            }
        }

        public Bullet Shoot()
        {
            return bullets.Dequeue();
        }
    }
}
