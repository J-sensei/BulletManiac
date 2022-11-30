using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.Player
{
    public class Gun : GameObject
    {

        public Gun(Texture2D texture)
        {
            name = "Player Gun";
            this.texture = texture;
            //scale = new Vector2(.5f, .5f);
        }
        protected override Rectangle CalculateBound()
        {
            return Rectangle.Empty;
        }
    }
}
