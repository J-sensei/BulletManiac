using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace BulletManiac.Entity
{
    /// <summary>
    /// Handle animation in different situationa
    /// </summary>
    public class AnimationManager
    {
        /// <summary>
        /// Link different action to an animation
        /// </summary>
        private readonly Dictionary<object, Animation> animations = new Dictionary<object, Animation>();
        /// <summary>
        /// Last used key
        /// </summary>
        private object lastKey;
        /// <summary>
        /// Current animation need to be draw
        /// </summary>
        public Animation CurrentAnimation { 
            get
            {
                return animations[lastKey];
            } 
        }

        /// <summary>
        /// Add new set of animation with the action key defined
        /// </summary>
        /// <param name="key"></param>
        /// <param name="animation"></param>
        public void AddAnimation(object key, Animation animation)
        {
            animations.Add(key, animation);
            lastKey ??= key; // Initialize the last key
        }

        /// <summary>
        /// Update the animation based on the key given
        /// </summary>
        /// <param name="key"></param>
        /// <param name="gameTime"></param>
        public void Update(object key, GameTime gameTime)
        {
            // If currently there is action key to call the animation, call it
            if (animations.ContainsKey(key))
            {
                animations[key].Start();
                animations[key].Update(gameTime);
                lastKey = key;
            }
            else // otherwise always refer to the last key to call the animation
            {
                animations[lastKey].Stop();
                animations[lastKey].Reset();
            }
        }
    }
}
