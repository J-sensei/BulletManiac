using BulletManiac.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.Enemy
{
    /// <summary>
    /// Hit box that will deal damage when colliding to the player
    /// </summary>
    public class HitBox : GameObject
    {
        private readonly float damage;
        private readonly Animation animation;
        private readonly List<int> frameToDrawBound;

        public HitBox(Animation animation, Vector2 position, Vector2 scale, List<int> frameToDrawBound, float damage = 50f)
        {
            name = "Hit Box";
            this.position = position;
            this.animation = animation;
            origin = animation.TextureBound / 2f;
            this.frameToDrawBound = frameToDrawBound;
            this.damage = damage;
            CollisionManager.Add(this, "Hit Box");
        }

        public override void Update(GameTime gameTime)
        {
            animation.Update(gameTime);
            if (animation.Finish)
            {
                Destroy(this);
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            DrawAnimation(animation, spriteBatch, gameTime);
        }

        protected override Rectangle CalculateBound()
        {
            if (frameToDrawBound.Contains(animation.CurrentFrameIndex))
            {
                Vector2 pos = position - origin;
                return new Rectangle((int)pos.X, (int)pos.Y, (int)animation.TextureBound.X, (int)animation.TextureBound.Y);
            }
            else
            {
                return Rectangle.Empty;
            }
        }

        public override void CollisionEvent(GameObject other)
        {
            if (other.Name == "Player")
            {
                Player.Player player = (other as Player.Player);
                player.TakeDamage(damage); // Test take damage
            }

            base.CollisionEvent(other);
        }
    }
}
