using BulletManiac.Managers;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletManiac.Entity.Bullet
{
    /// <summary>
    /// Generic bullet class
    /// </summary>
    public abstract class Bullet : GameObject
    {
        protected const float DEFAULT_SPEED = 100f;
        protected const float DEFAULT_BASED_DAMAGE = 10f;
        protected const float DEFAULT_DAMAGE_MULTIPLIER = 1f;
        protected const float DEFAULT_ANIMATION_SPEED = 0.2f;

        private Animation animation;
        protected float speed = 100f;

        /// <summary>
        /// Original damage of the bullet
        /// </summary>
        protected float basedDamage = DEFAULT_BASED_DAMAGE;
        /// <summary>
        /// Modify the base damage value
        /// </summary>
        protected float damageMultiplier = DEFAULT_DAMAGE_MULTIPLIER;
        /// <summary>
        /// How much damage deal to the enemy
        /// </summary>
        protected float damage;

        protected Animation Animation { get { return animation; } set { animation = value; } }

        public Bullet(Vector2 position, Vector2 direction, float speed = DEFAULT_SPEED)
        {
            name = "Bullet";
            this.position = position;
            Direction = direction;
            this.speed = speed;
        }

        public override void Initialize()
        {
            // Test
            //GameManager.Resources.LoadTexture("Test Bullet", "SpriteSheet/Bullet/Test_Bullets_16x16");
            //Rectangle b = new Rectangle(0, 16, 16, 16);
            //texture = GameManager.Resources.FindTexture("Test Bullet").CropTexture2D(b);
            //origin = new Vector2(8f);
            //scale = new Vector2(0.5f);
            base.Initialize();
        }

        //protected override Rectangle CalculateBound()
        //{
        //    //Vector2 pos = position - origin;
        //    //return new Rectangle((int)pos.X, (int)pos.Y, texture.Width, texture.Height);

        //    Vector2 pos = position - (origin * scale / 1.1f);
        //    return new Rectangle((int)pos.X, (int)pos.Y, (int)(texture.Width * scale.X / 1.25f), (int)(texture.Height * scale.Y));
        //}

        public override void Update(GameTime gameTime)
        {
            // Update the animation
            if(animation != null)
            {
                animation.Update(gameTime);
                texture = animation.CurrentTexture;
            }

            // Default bahavior of the bullet, which is moving to the desired direction
            position += Direction * speed * GameManager.DeltaTime; // Move the bullet
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
        }

        public override void CollisionEvent(GameObject other)
        {
            // Destroy the bullet
            // Destroy the enemy
            base.CollisionEvent(other);
        }
    }
}
