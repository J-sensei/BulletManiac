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
    internal class BulletCapacity : PowerUp
    {
        public BulletCapacity(Vector2 position) : base(ResourcesManager.FindTexture("Bullet_Capacity"), position)
        {
            origin = new Vector2(16f);
            scale = new Vector2(0.5f);
        }

        public override void Initialize()
        {
            soundEffect = ResourcesManager.FindSoundEffect("Mag_In");
            base.Initialize();
        }

        protected override Rectangle CalculateBound()
        {
            return new Rectangle((int)(position.X - (origin.X * scale.X)), (int)(position.Y - (origin.Y * scale.Y)), (int)(32 * scale.X), (int)(32 * scale.Y));
        }

        protected override void PowerUpAction()
        {
            GameManager.Player.Gun.AddCapacity(1);
        }
    }
}
