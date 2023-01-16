using BulletManiac.Collision;
using BulletManiac.Entity.UI;
using BulletManiac.Managers;
using BulletManiac.Particle;
using BulletManiac.SpriteAnimation;
using BulletManiac.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletManiac.Entity.PowerUps
{
    public abstract class PowerUp : GameObject
    {
        protected string powerUpName;
        protected SoundEffect soundEffect;
        private bool isPlay = false;
        protected TextureEffect shadowEffect;
        protected Animation animation;

        const float offset = 1.5f;
        Vector2 originalPos, upPos, downPos;
        bool moveUp = true;
        float moveSpeed = 10f;
        public PowerUp(Texture2D icon, Vector2 position) : base(icon)
        {
            powerUpName = "Unknown Power Up";
            name = "Power Up";
            this.position = position;
            originalPos = position;
            upPos = new Vector2(position.X, position.Y - offset);
            downPos = new Vector2(position.X, position.Y + offset);
            CollisionManager.Add(this, "Power Up");

        }
        protected Panel description;
        private bool showDescription = false;
        public override void Initialize()
        {
            // Dummy game object to rememeber the original position
            Button dummy = new Button(originalPos, Vector2.Zero);

            shadowEffect = new TextureEffect(ResourcesManager.FindTexture("Shadow"),
                                new Rectangle(0, 0, 64, 64), // Crop the shadow sprite
                                dummy,
                                new Vector2(32f), new Vector2(0.5f), new Vector2(0f, -3.5f));

            base.Initialize();
        }

        protected abstract void PowerUpAction();

        public override void CollisionEvent(ICollidable other)
        {
            if (other.Tag == "Player")
            {
                PowerUpAction();
                Destroy(this);

                // Play sound
                if (soundEffect != null && !isPlay)
                {
                    //soundEffect.Play();
                    AudioManager.Play(soundEffect);
                    isPlay = true;
                }

            }
            base.CollisionEvent(other);
        }

        public override void Update(GameTime gameTime)
        {
            shadowEffect.Update(gameTime);

            if (moveUp)
            {
                position.Y -= moveSpeed * Time.DeltaTime;
                if (position.Y < upPos.Y) moveUp = false;
            }
            else
            {
                position.Y += moveSpeed * Time.DeltaTime;
                if (position.Y > downPos.Y) moveUp = true;
            }

            Vector2 mousePos = Camera.ScreenToWorld(InputManager.MousePosition);
            Rectangle mouseRect = new Rectangle((int)mousePos.X, (int)mousePos.Y, 1, 1);

            if (Bound.Intersects(mouseRect))
                showDescription = true;
            else
                showDescription = false;

            if (animation != null)
                animation.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            shadowEffect.Draw(spriteBatch, gameTime);
            if (animation != null)
                DrawAnimation(animation, spriteBatch, gameTime);
            else
                base.Draw(spriteBatch, gameTime);

            if(description != null && showDescription)
                description.Draw(spriteBatch, gameTime);
        }
    }
}
