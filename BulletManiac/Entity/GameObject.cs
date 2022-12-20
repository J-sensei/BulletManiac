﻿using BulletManiac.Managers;
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

        private Texture2D debugBox; // Debug texture, used to draw the red box

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
        /// <summary>
        /// Determine whether the game object need to delete from the game
        /// </summary>
        private bool destroyed = false;
        /// <summary>
        /// Flip horizontally or vertically the texture sprite
        /// </summary>
        protected SpriteEffects spriteEffects = SpriteEffects.None;
        #endregion

        #region Properties
        /// <summary>
        /// Rectangle bound of the game object
        /// </summary>
        public Rectangle Bound { get { return CalculateBound(); } }
        /// <summary>
        /// Is the game object destroyed already?
        /// </summary>
        public bool IsDestroyed { get { return destroyed; } }
        /// <summary>
        /// Name of the game object
        /// </summary>
        public string Name { get { return name; } }
        /// <summary>
        /// Current position of the game object
        /// </summary>
        public Vector2 Position { get { return position; } set { position = value; } }
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
        
        public delegate void OnDestroy();
        public OnDestroy DestroyAction { get; set; }

        public GameObject(string name = DEFAULT_NAME)
        {
            this.name = name;

            // Initialize debug box
            debugBox = new Texture2D(GameManager.GraphicsDevice, 1, 1);
            debugBox.SetData(new Color[] { Color.Red });
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

            if (texture == null)
            {
                //GameManager.Log("Game Object", "\"" + Name + "\"" + " Texture is null, skipping to draw it");
                return;
            }
            spriteBatch.Draw(texture, position, null, Color.White, Rotation, origin, scale, spriteEffects, 0f);
        }

        /// <summary>
        /// Destroy the game object from the game
        /// </summary>
        /// <param name="gameObject"></param>
        public static void Destroy(GameObject gameObject)
        {
            if(gameObject.DestroyAction != null)
                gameObject.DestroyAction.Invoke(); // Invoke any destroy action

            gameObject.destroyed = true; // Entities Manager will take care of this after destroy is set to true
        }

        /// <summary>
        /// Trigger when collide with other game object
        /// </summary>
        /// <param name="other"></param>
        public virtual void CollisionEvent(GameObject other) { }

        public virtual void Dispose()
        {
            if(texture != null) texture.Dispose();
            if(debugBox != null) debugBox.Dispose();
        }
    }
}
