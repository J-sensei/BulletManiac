﻿using BulletManiac.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Particle
{
    public class AnimationEffect : GameObject
    {
        private Animation animation;
        private bool destroyWhenFinish = false;

        public AnimationEffect(Animation animation, Vector2 spawnPosition, Vector2 origin, bool destroyWhenFinish = true, SpriteEffects spriteEffects = 0)
        {
            this.animation = animation;
            this.destroyWhenFinish = destroyWhenFinish;
            position = spawnPosition;
            this.origin = origin;
            this.spriteEffects = spriteEffects;
            animation.Reset();
        }

        public override void Update(GameTime gameTime)
        {
            if(animation != null)
            {
                animation.Update(gameTime);
                texture = animation.CurrentTexture;
            }

            if (animation.Finish)
            {
                Destroy(this);
            }

            base.Update(gameTime);
        }

        protected override Rectangle CalculateBound()
        {
            return Rectangle.Empty;
        }

        public override void Dispose()
        {
            if(animation != null)
            {
                animation.Dispose();
                animation = null;
            }
            base.Dispose();
        }
    }
}
