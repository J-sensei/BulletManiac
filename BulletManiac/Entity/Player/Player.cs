using BulletManiac.Collision;
using BulletManiac.Managers;
using BulletManiac.Utilities;
using BulletManiac.Entity.Bullet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using BulletManiac.Particle;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace BulletManiac.Entity.Player
{
    public class Player : GameObject
    {
        /// <summary>
        /// Player actions, used to determine animation
        /// </summary>
        enum PlayerAction
        {
            Idle, Run, Walk, Death, Throw
        }

        private AnimationManager animationManager; // Manange the animation based on certain action
        private List<SoundEffect> footstepsSound;
        private TextureEffect shadowEffect; // Visual shadow effect

        public Gun Gun { get; private set; } // Player gun

        // Player status
        float moveSpeed = 80f;

        // Animation speed
        float animationSpeed = 0.2f;
        float idleAnimationSpeed = 0.2f;
        float walkAnimationSpeed = 0.2f;
        float runAnimationSpeed = 0.08f;
        float attackAnimationSpeed = 0.065f; // Attach speed
        float shootSpeed = 0.01f;

        // Accuracy
        const float DEFAULT_ACCURACY = 0.25f;
        float accuracy = DEFAULT_ACCURACY; // The lower the number, the better accuracy

        private bool moveBackward = false;
        private bool shooting = false;
        private PlayerAction currentAction = PlayerAction.Idle; // Current action of the player is doing

        public Player(Vector2 position)
        {
            name = "Player";
            this.position = position;
            animationManager = new AnimationManager(); // Using animation manager to handler different kind of animation
            scale = new Vector2(0.65f); // Scale of the player
            origin = new Vector2(16f); // Origin (Half of the sprite size) 32x32 / 2

            // Define the keys and animations
            animationManager.AddAnimation(PlayerAction.Idle, new Animation(GameManager.Resources.FindTexture("Player_SpriteSheet"), 2, 32, 32, idleAnimationSpeed));
            animationManager.AddAnimation(PlayerAction.Run, new Animation(GameManager.Resources.FindTexture("Player_SpriteSheet"), 8, 32, 32, runAnimationSpeed, 4));
            animationManager.AddAnimation(PlayerAction.Walk, new Animation(GameManager.Resources.FindTexture("Player_SpriteSheet"), 4, 32, 32, walkAnimationSpeed, 3));
            animationManager.AddAnimation(PlayerAction.Death, new Animation(GameManager.Resources.FindTexture("Player_SpriteSheet"), 8, 32, 32, animationSpeed, 8));
            animationManager.AddAnimation(PlayerAction.Throw, new Animation(GameManager.Resources.FindTexture("Player_SpriteSheet"), 8, 32, 32, attackAnimationSpeed * shootSpeed, 9, looping: false));

            footstepsSound = new List<SoundEffect>()
            {
                GameManager.Resources.FindSoundEffect("Footstep1"),
                GameManager.Resources.FindSoundEffect("Footstep2"),
                GameManager.Resources.FindSoundEffect("Footstep3"),
                GameManager.Resources.FindSoundEffect("Footstep5"),
                GameManager.Resources.FindSoundEffect("Footstep6"),
                GameManager.Resources.FindSoundEffect("Footstep7")
            };

            Texture2D shadowTexture = Extensions.CropTexture2D(GameManager.Resources.FindTexture("Shadow"), new Rectangle(0, 0, 64, 64)); // Crop a shadow texture
            shadowEffect = new TextureEffect(shadowTexture, this, shadowTexture.Bounds.Center.ToVector2(), new Vector2(0.5f), new Vector2(0f, -3.5f));

            Gun = new Gun(this);
            CollisionManager.Add(this, "Player");
        }

        protected override Rectangle CalculateBound()
        {
            if (texture == null) return Rectangle.Empty;
            // Left and right sprite will have slightly different bound to create accurate bound detection
            if(spriteEffects == SpriteEffects.None)
            {
                Vector2 pos = position - (origin * scale / 1.1f) + new Vector2(2f, 0f); ;
                return new Rectangle((int)pos.X, (int)pos.Y + 3, (int)(texture.Width * scale.X / 1.25f), (int)(texture.Height * scale.Y / 1.1f));
            }
            else
            {
                Vector2 pos = position - (origin * scale / 1.1f) + new Vector2(2f, 0f);
                return new Rectangle((int)pos.X, (int)pos.Y + 3, (int)(texture.Width * scale.X / 1.25f), (int)(texture.Height * scale.Y / 1.1f));
            }
        }

        private void PlayerAttack()
        {
            if (shooting && animationManager.GetAnimation(PlayerAction.Throw).Finish)
            {
                shooting = false;
                animationManager.GetAnimation(PlayerAction.Throw).Reset(); // Reset the animation once its finish playing
            }

            if (InputManager.MouseLeftHold && !shooting)
            {
                if (InputManager.GetKeyDown(Keys.LeftShift)) // Player is walking, more accurate shooting
                {
                    accuracy = DEFAULT_ACCURACY * 0.2f; // NEED TO CHANGE LATER
                }
                else
                {
                    accuracy = DEFAULT_ACCURACY;
                }

                shooting = true;
                currentAction = PlayerAction.Throw;
                GameManager.MainCamera.Shake();

                // Spawn Bullet
                Vector2 mousePos = Camera.ScreenToWorld(InputManager.MousePosition); // Convert mouse screen position to the world position
                Vector2 bulletDirection = mousePos - position;
                bulletDirection.Normalize();

                // Random the based on the accuracy of the player
                bulletDirection.X = Extensions.RandomRangeFloat(bulletDirection.X - accuracy, bulletDirection.X + accuracy);
                bulletDirection.Y = Extensions.RandomRangeFloat(bulletDirection.Y - accuracy, bulletDirection.Y + accuracy);

                // Fire Bullet
                DefaultBullet bullet = new DefaultBullet(position, bulletDirection, 100f);
                GameManager.AddGameObject(bullet); // Straight away add bullet to entity manager to run it immediately
            }
        }

        private void PlayerMovement()
        {
            // Flip the sprite based on the mouse X position
            Vector2 mousePos = Camera.ScreenToWorld(InputManager.MousePosition);
            if (position.X <= mousePos.X)
            {
                spriteEffects = SpriteEffects.None;
            }
            else if (position.X > mousePos.X)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }

            if (shooting) return; // No movement when player is shooting

            // Updating the speed
            if (InputManager.Moving)
            {
                float currentSpeed = 0f;

                // Player moving backward, set animation reverse
                if (spriteEffects == SpriteEffects.None && InputManager.Direction.X < 0 ||
                   spriteEffects == SpriteEffects.FlipHorizontally && InputManager.Direction.X > 0)
                {
                    animationManager.GetAnimation(PlayerAction.Run).SetReverse(true);
                    moveBackward = true;
                }
                else
                {
                    animationManager.GetAnimation(PlayerAction.Run).SetReverse(false);
                    moveBackward = false;
                }

                // Player Move
                if (InputManager.GetKeyDown(Keys.LeftShift)) // Player is walking, more accurate shooting
                {
                    currentSpeed = moveSpeed * 0.7f; // NEED TO CHANGE LATER
                    currentAction = PlayerAction.Walk;
                }
                else // Player is running (Default), less accrate shooting
                {
                    currentSpeed = moveSpeed;
                    currentAction = PlayerAction.Run;
                }
                
                ApplyMove(InputManager.Direction, currentSpeed); // Move the player and apply the collision to tiles
                WalkingSFX(); // Update Moving SFX
            }
            else
            {
                currentAction = PlayerAction.Idle; // Player is Idle when not moving
            }
        }

        private void ApplyMove(Vector2 direction, float moveSpeed)
        {
            bool moveX = false; // Determine if the player can move left / right
            bool moveY = false; // Determine if the player can move up / down

            Vector2 moveAmount = Vector2.Normalize(direction) * moveSpeed * GameManager.DeltaTime; // Amount of move in this frmae
            Vector2 moveAmountX = moveAmount; // Amount of move for x axis
            moveAmountX.Y = 0;
            Vector2 moveAmountY = moveAmount; // Amount of move for y axis
            moveAmountY.X = 0;

            // Check the collisioni for x and y axis
            if(direction.X >= 0)
                moveX = !CollisionManager.CheckTileCollision(this, moveAmountX * 1.5f);
            else
                moveX = !CollisionManager.CheckTileCollision(this, moveAmountX * 1f);

            if(direction.Y >= 0)
                moveY = !CollisionManager.CheckTileCollision(this, moveAmountY * 1.5f);
            else
                moveY = !CollisionManager.CheckTileCollision(this, moveAmountY * 1f);
    
            // Move the character according to the result
            if (moveX)
                position.X += moveAmountX.X;

            if (moveY)
                position.Y += moveAmountY.Y;
        }

        int lastWalkingAnimIndex = 0; // Play the walking sfx only once
        List<int> walkFrame = new List<int>() { 0, 2}; // Frame to generate SFX (Walk)
        List<int> runFrame = new List<int>() { 2, 6 }; // Frame to generate SFX (Run)
        private void WalkingSFX()
        {
            Animation anim;
            if (currentAction == PlayerAction.Run)
            {
                anim = animationManager.GetAnimation(PlayerAction.Run);
                CreateWalkSFX(anim, runFrame);
            }
            else if(currentAction == PlayerAction.Walk)
            {
                anim = animationManager.GetAnimation(PlayerAction.Walk);
                CreateWalkSFX(anim, walkFrame);
            }
        }

        /// <summary>
        /// Take list of frame that need to generate effect and sound
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="frameToCreate"></param>
        private void CreateWalkSFX(Animation animation, List<int> frameToCreate)
        {
            int currentIndex = animation.CurrentFrameIndex;
            if (!frameToCreate.Contains(currentIndex) || currentIndex == lastWalkingAnimIndex) return;

            // Smoke Effect
            TextureEffect effect;
            SpriteEffects smokeSpriteEffects = SpriteEffects.None;
            if (spriteEffects != SpriteEffects.None)
            {
                if (!moveBackward) smokeSpriteEffects = SpriteEffects.FlipHorizontally;
                effect = new TextureEffect(new Animation(GameManager.Resources.FindTexture("Walking_Smoke"), 6, 1, 0.1f, looping: false),
                        Position + new Vector2(8f, 5f), new Vector2(32, 32), new Vector2(1f), true, smokeSpriteEffects);
            }
            else
            {
                if (moveBackward) smokeSpriteEffects = SpriteEffects.FlipHorizontally;
                effect = new TextureEffect(new Animation(GameManager.Resources.FindTexture("Walking_Smoke"), 6, 1, 0.1f, looping: false),
                        Position + new Vector2(-5f, 5f), new Vector2(32, 32), new Vector2(1f), true, smokeSpriteEffects);
            }

            GameManager.AddGameObject(effect);

            // Audio
            footstepsSound[Extensions.Random.Next(footstepsSound.Count)].Play();
            lastWalkingAnimIndex = currentIndex;
        }

        public override void Initialize()
        {
            CollisionManager.Add(this, "Player"); // Add player into the collision manager
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            PlayerMovement();
            // PlayerAttack(); // Now player is using gun to shoot
            Gun.Update(gameTime);

            // Update the animations
            animationManager.Update(currentAction, gameTime);
            texture = animationManager.CurrentAnimation.CurrentTexture; // Update the texture based on the animation
            shadowEffect.Update(gameTime);
            base.Update(gameTime);
        }

        Vector2 textOffset = new Vector2(32f, 0f);
        Vector2 textPosOffset = new Vector2(0f, -16f);
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            shadowEffect.Draw(spriteBatch, gameTime); // Shadow always behind the player
            // Draw the gun and player
            if (Gun.RenderInfront)
            {
                base.Draw(spriteBatch, gameTime);
                Gun.Draw(spriteBatch, gameTime);
            }
            else
            {
                Gun.Draw(spriteBatch, gameTime);
                base.Draw(spriteBatch, gameTime);
            }

            // Reloading Text
            if(Gun.Reloading)
                spriteBatch.DrawString(GameManager.Resources.FindSpriteFont("DebugFont"), "Reloading...", position + textPosOffset, Color.White, 0f, textOffset, 0.3f, SpriteEffects.None, 0f);
            
            //animationManager.CurrentAnimation.Draw(spriteBatch, position, Color.White, 0f, origin, new Vector2(3f, 3f), SpriteEffects.None, 0f);
        }

        public override void CollisionEvent(GameObject gameObject)
        {

        }
    }
}
