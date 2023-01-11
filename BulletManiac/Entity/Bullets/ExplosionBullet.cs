using BulletManiac.Entity.Enemies;
using BulletManiac.Managers;
using BulletManiac.SpriteAnimation;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.Bullets
{
    public class ExplosionBullet : Bullet
    {
        public ExplosionBullet() : base(ResourcesManager.FindSoundEffect("Explosion_Shoot"))
        {
            Animation = new Animation(ResourcesManager.FindAnimation("ExplosionBullet_Animation"));
            Animation.Reset();

            basedDamage = 0;
            origin = Animation.TextureBound / 2f; // Set the origin to the center of the texture
            scale = new Vector2(0.8f);
            BulletUI = ResourcesManager.FindTexture("Bullet_Explosion");
        }

        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - origin * scale / 1.2f;
            return new Rectangle((int)pos.X + 2, (int)pos.Y + 3, (int)(origin.X * 2 * scale.X / 1.5f), (int)(origin.Y * 2 * scale.Y / 1.5f));
        }

        void CreateExplosion()
        {
            HitBox hitBox = new HitBox(new Animation(ResourcesManager.FindAnimation("BulletExplode_Animation")),
            Position, new Vector2(1f), new List<int>() { 3, 4 }, damage: 30f, enableEnemyDamage: true, enablePlayerDamage: false);
            hitBox.AddSoundEffect(ResourcesManager.FindSoundEffect("SuicideShadow_Explosion"), 1); // Add new sound effect later
            GameManager.AddGameObject(hitBox);

            // Shake the camera is the explosion is happen inside the visible area
            if (Camera.Main.InViewBound(Position))
                Camera.Main.Shake(2f);
        }

        protected override void DeleteEventWall()
        {
            CreateExplosion();
            IsDestroyed = true;
        }

        public override void DeleteEvent()
        {
            CreateExplosion();
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
