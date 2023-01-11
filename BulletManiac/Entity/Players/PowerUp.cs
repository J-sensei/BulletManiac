using BulletManiac.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.Players
{
    public abstract class PowerUp : GameObject
    {
        public PowerUp(Texture2D icon) : base(icon)
        {
            
        }

        protected abstract void PowerUpAction();

        protected override Rectangle CalculateBound()
        {
            throw new NotImplementedException();
        }

        public override void CollisionEvent(ICollidable other)
        {
            if(other.Tag == "Player")
            {
                PowerUpAction();
                Destroy(this);
                // Play sound
            }
            base.CollisionEvent(other);
        }
    }
}
