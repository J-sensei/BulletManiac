using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletManiac.Entity
{
    /// <summary>
    /// Base object of all entities in the game
    /// </summary>
    public abstract class GameObject
    {
        #region Constant
        const string DEFAULT_NAME = "New GameObject";
        #endregion

        #region Data fields
        /// <summary>
        /// Name of the game object
        /// </summary>
        protected string name;
        /// <summary>
        /// Texture will be draw onto the screen
        /// </summary>
        protected Texture2D texture;
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

        private bool destroyed = false;
        #endregion

        #region Properties
        public Rectangle Bound
        {
            get
            {
                return CalculateBound();
            }
        }
        public bool IsDestroyed
        {
            get { return destroyed; }
        }
        public string Name { get { return name; } }
        #endregion

        public delegate void OnCollision(GameObject gameObject);
        public delegate void OnDestroy();
        /// <summary>
        /// Action to happens when this object collide with other object
        /// </summary>
        public OnCollision CollisionAction;
        public OnDestroy DestroyAction;

        public GameObject(string name = DEFAULT_NAME)
        {
            this.name = name;
        }

        protected abstract Rectangle CalculateBound();

        public virtual void Initialize() { }

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, scale, SpriteEffects.None, 0f);
        }

        public static void Destroy(GameObject gameObject)
        {
            gameObject.destroyed = true;
        }
    }
}
