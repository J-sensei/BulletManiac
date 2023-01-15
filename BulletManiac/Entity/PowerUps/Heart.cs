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
    internal class Heart : PowerUp
    {
        const float heal = 20f;
        public Heart(Vector2 position) : base(ResourcesManager.FindTexture("PowerUp_Heart"), position)
        {
            origin = new Vector2(8.5f);
            scale = new Vector2(1f);
        }

        public override void Initialize()
        {
            soundEffect = ResourcesManager.FindSoundEffect("Pause");
            animation = new Animation(ResourcesManager.FindTexture("PowerUp_Heart_Animated"), 5, 1, 0.1f);
            description = new Panel(new Vector2(position.X, position.Y - 10f), new Vector2(3.5f, 1f), "Heal 20 HP");
            base.Initialize();
        }

        protected override Rectangle CalculateBound()
        {
            return new Rectangle((int)(position.X - (origin.X * scale.X)), (int)(position.Y - (origin.Y * scale.Y)), (int)(17 * scale.X), (int)(17 * scale.Y));
        }

        protected override void PowerUpAction()
        {
            GameManager.Player.Heal(heal);
        }
    }
}
