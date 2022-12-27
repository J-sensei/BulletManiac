using BulletManiac.Entity.Player;
using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.UI
{
    public class Magazine
    {
        class BulletUI
        {
            private readonly Texture2D fullTexture;
            private readonly Texture2D emptyTexture;
            private bool isEmpty;

            public Texture2D Texture { 
                get
                {
                    if (isEmpty) return emptyTexture;
                    else return fullTexture;
                } 
            }

            public bool Empty { set { isEmpty = value; } }

            public BulletUI(Texture2D fullTexture, Texture2D emptyTexture, bool isEmpty = false)
            {
                this.fullTexture = fullTexture;
                this.emptyTexture = emptyTexture;
                this.isEmpty = isEmpty;
            }
        }

        private int capacity = 5;
        private BulletUI bulletUI;

        public Magazine(Gun gun)
        {
            bulletUI = new BulletUI(GameManager.Resources.FindTexture("Bullet_Full"), GameManager.Resources.FindTexture("Bullet_Empty"));
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw(SpriteBatch spriteBarch, GameTime gameTime)
        {

        }
    }
}
