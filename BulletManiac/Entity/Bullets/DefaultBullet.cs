using BulletManiac.Managers;
using BulletManiac.SpriteAnimation;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.Bullets
{
    /// <summary>
    /// Most normal bullet in the game (Only shoot straight line)
    /// </summary>
    public class DefaultBullet : Bullet
    {
        public DefaultBullet() : base()
        {
            //Animation = new Animation(GameManager.Resources.FindTexture("Bullet1"), 5, 25, 0.1f, 6);
            //Animation = GameManager.Resources.FindAnimation("DefaultBullet_Animation");
            Animation = new Animation(ResourcesManager.FindAnimation("DefaultBullet_Animation"));
            Animation.Reset();

            origin = new Vector2(8f); // Set the origin to the center of the texture
            scale = new Vector2(0.8f);
            BulletUI = ResourcesManager.FindTexture("Bullet_Fill");
        }

        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - origin * scale / 1.2f;
            return new Rectangle((int)pos.X + 2, (int)pos.Y + 3, (int)(origin.X * 2 * scale.X / 1.5f), (int)(origin.Y * 2 * scale.Y / 1.5f));
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
