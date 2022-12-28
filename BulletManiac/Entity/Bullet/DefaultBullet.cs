using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.Bullet
{
    /// <summary>
    /// Most normal bullet in the game (Only shoot straight line)
    /// </summary>
    public class DefaultBullet : Bullet
    {
        public DefaultBullet(Vector2 position, Vector2 direction, float speed = DEFAULT_SPEED, float initalSpeed = 0f) : base(position, direction, speed, initalSpeed)
        {
            Animation = new Animation(GameManager.Resources.FindTexture("Bullet1"), 5, 25, 0.1f, 6);
            Animation.Reset();

            origin = Animation.CurrentTexture.Bounds.Center.ToVector2(); // Set the origin to the center of the texture
            scale = new Vector2(0.8f);
            BulletUI = GameManager.Resources.FindTexture("Bullet_Fill");
        }

        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - (origin * scale / 1.2f);
            if(texture != null)
            {
                return new Rectangle((int)pos.X + 2, (int)pos.Y + 3, (int)(texture.Width * scale.X / 1.5f), (int)(texture.Height * scale.Y / 1.5f));
            }
            else
            {
                return Rectangle.Empty;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
