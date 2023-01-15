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
    internal class Heart : PowerUp
    {
        public Heart(Vector2 position) : base(ResourcesManager.FindTexture("PowerUp_Heart"), position)
        {
            origin = new Vector2(8.5f);
            scale = new Vector2(1f);
        }

        public override void Initialize()
        {
            soundEffect = ResourcesManager.FindSoundEffect("Pause");
            animation = new Animation(ResourcesManager.FindTexture("PowerUp_Heart_Animated"), 5, 1, 0.1f);
            base.Initialize();
        }

        protected override Rectangle CalculateBound()
        {
            return new Rectangle((int)(position.X - (origin.X * scale.X)), (int)(position.Y - (origin.Y * scale.Y)), (int)(17 * scale.X), (int)(17 * scale.Y));
        }

        protected override void PowerUpAction()
        {
            GameManager.Player.Heal(40f);
        }
    }
}
