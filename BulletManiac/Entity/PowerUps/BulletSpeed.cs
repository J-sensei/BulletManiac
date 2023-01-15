using BulletManiac.Entity.Bullets;
using BulletManiac.Entity.UI;
using BulletManiac.Managers;
using BulletManiac.SpriteAnimation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.PowerUps
{
    internal class BulletSpeed : PowerUp
    {
        const float MULTIPLIER = 0.1f;
        public BulletSpeed(Vector2 position) : base(ResourcesManager.FindTexture("Bullet_Speed"), position)
        {
            origin = new Vector2(16f);
            scale = new Vector2(0.5f);
        }

        public override void Initialize()
        {
            soundEffect = ResourcesManager.FindSoundEffect("Mag_In");
            description = new Panel(new Vector2(position.X, position.Y - 10f), new Vector2(9f, 1f), "Increase 10% bullet speed");
            base.Initialize();
        }

        protected override Rectangle CalculateBound()
        {
            return new Rectangle((int)(position.X - (origin.X * scale.X)), (int)(position.Y - (origin.Y * scale.Y)), (int)(32 * scale.X), (int)(32 * scale.Y));
        }

        protected override void PowerUpAction()
        {
            Bullet.SpeedModifier = Bullet.SpeedModifier + MULTIPLIER;
        }
    }
}
