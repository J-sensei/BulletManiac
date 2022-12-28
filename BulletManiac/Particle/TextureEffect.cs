using BulletManiac.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Particle
{
    /// <summary>
    /// Generate particle effect using texture or animation
    /// </summary>
    public class TextureEffect : GameObject
    {
        // Static Texture Effect
        private Vector2 offset; // Draw offset of the texture
        private GameObject parent;

        // Animated Effect
        private Animation animation;
        private bool destroyWhenFinish = false;

        public TextureEffect(Animation animation, Vector2 spawnPosition, Vector2 origin, Vector2 scale, bool destroyWhenFinish = true, SpriteEffects spriteEffects = 0)
        {
            this.name = "Animated Texture Effect";
            this.animation = animation;
            this.destroyWhenFinish = destroyWhenFinish;
            position = spawnPosition;
            this.origin = origin;
            this.spriteEffects = spriteEffects;
            this.scale = scale;
            animation.Reset();
        }

        public TextureEffect(Texture2D texture, GameObject parent, Vector2 origin, Vector2 scale, Vector2 offset, SpriteEffects spriteEffects = 0)
        {
            this.name = "Texture Effect";
            this.texture = texture;
            this.parent = parent;
            position = parent.Position;
            this.origin = origin;
            this.scale = scale;
            this.offset = offset;
            this.spriteEffects = spriteEffects;
        }

        public override void Update(GameTime gameTime)
        {
            if(animation != null)
            {
                animation.Update(gameTime);
                texture = animation.CurrentTexture;

                if (animation.Finish)
                {
                    Destroy(this);
                }
            }

            if(parent != null)
            {
                Position = parent.Position + offset;
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
