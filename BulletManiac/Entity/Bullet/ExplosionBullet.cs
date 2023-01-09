using BulletManiac.Entity.Enemy;
using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.Bullet
{
    public class ExplosionBullet : Bullet
    {
        public ExplosionBullet() : base()
        {
            Animation = new Animation(GameManager.Resources.FindAnimation("ExplosionBullet_Animation"));
            Animation.Reset();

            origin = Animation.TextureBound / 2f; // Set the origin to the center of the texture
            scale = new Vector2(0.8f);
            BulletUI = GameManager.Resources.FindTexture("Bullet_Fill");
        }

        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - (origin * scale / 1.2f);
            return new Rectangle((int)pos.X + 2, (int)pos.Y + 3, (int)((origin.X * 2) * scale.X / 1.5f), (int)((origin.Y * 2) * scale.Y / 1.5f));
        }

        void CreateExplosion()
        {
            HitBox hitBox = new HitBox(new Animation(GameManager.Resources.FindAnimation("BulletExplode_Animation")),
            Position, new Vector2(1f), new List<int>() { 3, 4 }, enableEnemyDamage: true, enablePlayerDamage: false);
            //hitBox.AddSoundEffect(GameManager.Resources.FindSoundEffect("SuicideShadow_Explosion"), 1); // Add new sound effect later
            GameManager.AddGameObject(hitBox);
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
