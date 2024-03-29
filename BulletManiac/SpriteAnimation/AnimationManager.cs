﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BulletManiac.SpriteAnimation
{
    /// <summary>
    /// Handle animation in different situations
    /// </summary>
    public class AnimationManager : IDisposable
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
        public Animation CurrentAnimation
        {
            get
            {
                return animations[lastKey];
            }
        }

        private bool active = true;

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
        /// Get the animation object based on the key given
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Animation GetAnimation(object key)
        {
            if (animations.ContainsKey(key))
                return animations[key];
            else
                return null;
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

                if (active)
                    animations[key].Update(gameTime);
                else
                    animations[key].Reset();

                lastKey = key;
            }
            else // otherwise always refer to the last key to call the animation
            {
                animations[lastKey].Stop();
                animations[lastKey].Reset();
            }
        }

        public void Start() => active = true;
        public void Stop() => active = false;

        public void Dispose()
        {
            foreach (var animation in animations.Values)
            {
                animation.Dispose();
            }
            animations.Clear();
        }
    }
}
