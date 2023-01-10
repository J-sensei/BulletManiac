using BulletManiac.Managers;
using BulletManiac.SpriteAnimation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Entity
{
    /// <summary>
    /// Base object of all entities in the game
    /// </summary>
    public abstract class GameObject : IDisposable
    {
        #region Constant
        const string DEFAULT_NAME = "New GameObject";
        #endregion

        private static Texture2D debugBox; // Debug texture, used to draw the red box

        #region Data fields
        /// <summary>
        /// Name of the game object
        /// </summary>
        protected string name;
        /// <summary>
        /// Texture will be draw onto the screen
        /// </summary>
        protected readonly Texture2D texture;
        /// <summary>
        /// Current position of the game object
        /// </summary>
        protected Vector2 position;
        /// <summary>
        /// Direction of the object facing
        /// </summary>
        protected Vector2 direction;
        /// <summary>
        /// Use to determine whether to calculate the rotation angle
        /// </summary>
        private bool directionChange = false;
        /// <summary>
        /// Current rotation to render the game object;
        /// </summary>
        private float rotation = 0f;
        /// <summary>
        /// Scale of the object should render
        /// </summary>
        protected Vector2 scale = Vector2.One;
        /// <summary>
        /// Origin point (pivot) of the object, default to top left corner
        /// </summary>
        protected Vector2 origin = Vector2.Zero;
        /// <summary>
        /// Determine whether the game object need to delete from the game
        /// </summary>
        private bool destroyed = false;
        /// <summary>
        /// Flip horizontally or vertically the texture sprite
        /// </summary>
        protected SpriteEffects spriteEffects = SpriteEffects.None;
        protected Color color = Color.White;
        protected float layerDepth = 0f;
        #endregion

        #region Properties
        /// <summary>
        /// Rectangle bound of the game object
        /// </summary>
        public Rectangle Bound { get { return CalculateBound(); } }
        /// <summary>
        /// Is the game object destroyed already?
        /// </summary>
        public bool IsDestroyed { get { return destroyed; } protected set { destroyed = value; } }
        /// <summary>
        /// Name of the game object
        /// </summary>
        public string Name { get { return name; } }
        /// <summary>
        /// Current position of the game object
        /// </summary>
        public Vector2 Position { get { return position; } set { position = value; } }
        /// <summary>
        /// Rotation of the game object (Radian)
        /// </summary>
        public float Rotation { 
            get 
            {
                // Atan2 is expensive, so only calculate when needed
                if (directionChange)
                {
                    rotation = MathF.Atan2(direction.Y, direction.X);
                    directionChange = false;
                }
                return rotation;
            }
        }
        /// <summary>
        /// Direction / Heading of the game object, normalized value
        /// </summary>
        public Vector2 Direction
        {
            get => direction;
            set
            {
                direction = value;
                // We ALWAYS normalize direction
                direction.Normalize();

                // Also, when direction is changed, we flag it
                // to ensure rotation is recalculated.
                directionChange = true;
            }
        }
        #endregion

        public GameObject(string name = DEFAULT_NAME)
        {
            this.name = name;

            // Initialize debug box
            if(debugBox == null)
            {
                debugBox = new Texture2D(GameManager.GraphicsDevice, 1, 1);
                debugBox.SetData(new Color[] { Color.Red });
            }
        }

        public GameObject(Texture2D texture, string name = DEFAULT_NAME)
        {
            this.texture = texture;
        }

        /// <summary>
        /// Calculate rectangle bound of the game object (Might be various across different game object)
        /// </summary>
        /// <returns></returns>
        protected abstract Rectangle CalculateBound();

        public virtual void Initialize() { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (GameManager.Debug)
            {
                spriteBatch.Draw(debugBox, Bound, Color.White); // Draw the debug red box
            }

            if (texture == null) return;
            
            spriteBatch.Draw(texture, position, null, color, Rotation, origin, scale, spriteEffects, layerDepth);
        }

        protected void DrawAnimation(Animation animation, SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (GameManager.Debug)
                spriteBatch.Draw(debugBox, Bound, Color.White); // Draw the debug red box

            animation.Draw(spriteBatch, position, color, Rotation, origin, scale, spriteEffects, layerDepth);
        }

        protected void DrawTexture(Rectangle uvBound, SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(texture, position, uvBound, color, Rotation, origin, scale, spriteEffects, layerDepth);
        }

        /// <summary>
        /// Destroy the game object from the game
        /// </summary>
        /// <param name="gameObject"></param>
        public static void Destroy(GameObject gameObject)
        {
            gameObject.DeleteEvent();
            gameObject.destroyed = true; // Entities Manager will take care of this after destroy is set to true
        }

        /// <summary>
        /// Trigger when collide with other game object
        /// </summary>
        /// <param name="other"></param>
        public virtual void CollisionEvent(GameObject other) { }
        /// <summary>
        /// Trigger before the object is delete
        /// </summary>
        public virtual void DeleteEvent() { }

        public virtual void Dispose()
        {
            
        }
    }
}
