using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Particle
{
    public class FadeItem
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Scale
        {
            get
            {
                if (delay > 0)
                    return 2.0f;
                else if (Radian > MathHelper.Pi)
                    return 0f;

                return MathF.Cos(Radian) + 1;
            }
        }
        public float Delay { get; set; }
        public float Radian { get; set; } = MathHelper.Pi;
        private float delay;

        public void Reset()
        {
            delay = Delay;
            Radian = 0f;
        }

        public void Update(float deltaTime)
        {
            delay -= deltaTime;

            if(delay < 0f)
            {
                Radian += (deltaTime / 200f);
            }
        }
    }
}
