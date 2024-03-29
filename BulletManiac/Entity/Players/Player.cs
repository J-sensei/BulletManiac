﻿using BulletManiac.Collision;
using BulletManiac.Managers;
using BulletManiac.Utilities;
using BulletManiac.Entity.Bullets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using BulletManiac.Particle;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using BulletManiac.SpriteAnimation;
using BulletManiac.Entity.UI;

namespace BulletManiac.Entity.Players
{
    /// <summary>
    /// Player actions, used to determine animation
    /// </summary>
    public enum PlayerAction
    {
        Idle, Run, Walk, Dash, Death
    }

    public class Player : GameObject
    {
        private ProgressBar hpBar;
        private AnimationManager animationManager; // Manange the animation based on certain action
        private List<SoundEffect> footstepsSounds; // Footstep sounds to play when walking
        private TextureEffect shadowEffect; // Visual shadow effect                           
        private Effect colorOverlay; // Shader for the blink effect

        // Player status
        private float moveSpeed = 80f;
        private float dashSpeed = 240f;
        const float DEFAULT_HP = 100f;
        public float HP { get; private set; }
        public float MaxHP { get; private set; }
        public bool IsGameOver { get; private set; } = false;

        // Animation speed
        private float animationSpeed = 0.2f;
        private float idleAnimationSpeed = 0.2f;
        private float walkAnimationSpeed = 0.2f;
        private float runAnimationSpeed = 0.08f;
        private float dashAnimationSpeed = 0.08f;

        private bool moveBackward = false; // Determine if player is moving backward while facing forward
        private PlayerAction currentAction = PlayerAction.Idle; // Current action of the player is doing
        public PlayerAction CurrentAction { get => currentAction; }

        // Invincible variables
        private bool invincible = false;
        private const float INVINCIBLE_TIME = 1f;
        private float currentInvincibleTime = INVINCIBLE_TIME;

        // Blink variables
        private bool blink = false; // Determine whether the character should apply blink effect
        private const float BLINK_TIME = 0.15f; // How fast blinking is render during after player take damage
        private float blinkToggleTime = BLINK_TIME;

        // Text position and offset to render with the player
        private Vector2 textOffset = new Vector2(32f, 0f);
        private Vector2 textPosOffset;

        // Dashing variables
        private Vector2 dashDirection;
        private bool dashing = false;
        private float dashCD = 0f;

        public Gun Gun { get; private set; } // Player gun

        public Player(Vector2 position)
        {
            name = "Player";
            this.position = position;
            animationManager = new AnimationManager(); // Using animation manager to handler different kind of animation
            scale = new Vector2(0.65f); // Scale of the player
            origin = new Vector2(16f); // Origin (Half of the sprite size) 32x32 / 2
            HP = DEFAULT_HP;
            MaxHP = DEFAULT_HP;

            Gun = new Gun(this);
        }

        protected override Rectangle CalculateBound()
        {
            Vector2 pos = position - origin * scale / 1.1f + new Vector2(2f, 0f);
            return new Rectangle((int)pos.X, (int)pos.Y + 3, (int)(origin.X * 2 * scale.X / 1.25f), (int)(origin.Y * 2 * scale.Y / 1.1f));
        }

        private void Dash()
        {
            if (InputManager.MouseRightClick && !dashing && dashCD <= 0f)
            {
                currentAction = PlayerAction.Dash;
                dashing = true;
                if (InputManager.Moving)
                {
                    dashDirection = InputManager.Direction;
                }
                else
                {
                    if (spriteEffects == SpriteEffects.None)
                        dashDirection.X = 1f;
                    else
                        dashDirection.X = -1f;

                    float offset = 150f;
                    if (InputManager.MousePosition.Y > GameManager.CurrentResolution.Y / 2f + offset)
                        dashDirection.Y = 1f;
                    else if (InputManager.MousePosition.Y < GameManager.CurrentResolution.Y / 2f - offset)
                        dashDirection.Y = -1f;
                    else
                        dashDirection.Y = 0f;
                }
                dashDirection.Normalize();
                //ResourcesManager.FindSoundEffect("Dash").Play();
                AudioManager.Play("Dash");
            }

            if (dashing)
            {
                ApplyMove(dashDirection, dashSpeed); // Move to the dashDirection when dashing

                if (animationManager.CurrentAnimation.Finish)
                {
                    dashing = false;
                    animationManager.GetAnimation(PlayerAction.Dash).Reset();
                    dashCD = 0.5f;
                }
            }
            else
            {
                if (dashCD > 0f)
                    dashCD -= Time.DeltaTime;
            }
        }

        private void PlayerMovement()
        {
            // Flip the sprite based on the mouse X position
            Vector2 mousePos = Camera.ScreenToWorld(InputManager.MousePosition);
            if (position.X < mousePos.X)
                spriteEffects = SpriteEffects.None;
            else if (position.X > mousePos.X)
                spriteEffects = SpriteEffects.FlipHorizontally;

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
                //Vector2 move = Vector2.Normalize(InputManager.Direction) * GameManager.DeltaTime * currentSpeed;
                //Position += CollisionManager.Move(Bound, move);
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

            Vector2 moveAmount = Vector2.Normalize(direction) * moveSpeed * Time.DeltaTime; // Amount of move in this frmae
            Vector2 moveAmountX = moveAmount; // Amount of move for x axis
            moveAmountX.Y = 0;
            Vector2 moveAmountY = moveAmount; // Amount of move for y axis
            moveAmountY.X = 0;

            // Check the collisioni for x and y axis
            //if(direction.X >= 0)
            //    moveX = !CollisionManager.CheckTileCollision(this, moveAmountX * 3f);
            //else
            //    moveX = !CollisionManager.CheckTileCollision(this, moveAmountX * 3f);

            //if(direction.Y >= 0)
            //    moveY = !CollisionManager.CheckTileCollision(this, moveAmountY * 3f);
            //else
            //    moveY = !CollisionManager.CheckTileCollision(this, moveAmountY * 3f);
            moveX = !CollisionManager.CheckTileCollision(this, moveAmountX * 3f);
            moveY = !CollisionManager.CheckTileCollision(this, moveAmountY * 3f);
            // Move the character according to the result
            if (moveX)
                position.X += moveAmountX.X;

            if (moveY)
                position.Y += moveAmountY.Y;
        }

        private int lastWalkingAnimIndex = 0; // Play the walking sfx only once
        private List<int> walkFrame = new List<int>() { 0, 2 }; // Frame to generate SFX (Walk)
        private List<int> runFrame = new List<int>() { 2, 6 }; // Frame to generate SFX (Run)
        private void WalkingSFX()
        {
            Animation anim;
            if (currentAction == PlayerAction.Run)
            {
                anim = animationManager.GetAnimation(PlayerAction.Run);
                CreateWalkSFX(anim, runFrame);
            }
            else if (currentAction == PlayerAction.Walk)
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
            Animation smokeAnim = new Animation(ResourcesManager.FindAnimation("Walking_Smoke_Animation"));
            if (spriteEffects != SpriteEffects.None)
            {
                if (!moveBackward) smokeSpriteEffects = SpriteEffects.FlipHorizontally;
                effect = new TextureEffect(smokeAnim,
                        Position + new Vector2(8f, 5f), new Vector2(32, 32), new Vector2(1f), true, smokeSpriteEffects);
            }
            else
            {
                if (moveBackward) smokeSpriteEffects = SpriteEffects.FlipHorizontally;
                effect = new TextureEffect(smokeAnim,
                        Position + new Vector2(-5f, 5f), new Vector2(32, 32), new Vector2(1f), true, smokeSpriteEffects);
            }
            GameManager.AddGameObject(effect); // Add smoke to the world

            // Audio
            //footstepsSounds[Extensions.Random.Next(footstepsSounds.Count)].Play();
            AudioManager.Play(footstepsSounds[Extensions.Random.Next(footstepsSounds.Count)]);
            lastWalkingAnimIndex = currentIndex;
        }

        private void Invincible()
        {
            if (invincible)
            {
                currentInvincibleTime -= Time.DeltaTime;
                blinkToggleTime -= Time.DeltaTime;

                // Update blink effect
                if (blinkToggleTime <= 0f)
                {
                    blink = !blink;
                    blinkToggleTime = BLINK_TIME;
                }

                // Invisible time finish
                if (currentInvincibleTime <= 0f)
                {
                    invincible = false;
                    currentInvincibleTime = INVINCIBLE_TIME;
                }
            }
        }

        public void TakeDamage(float damage)
        {
            // If player is not in invincible 
            if (!invincible && !dashing)
            {
                HP -= damage;
                HP = MathHelper.Clamp(HP, 0f, MaxHP); // Clamp HP
                invincible = true; // Player become invincible after damage is taken
                //ResourcesManager.FindSoundEffect("Player_Hurt").Play();
                AudioManager.Play("Player_Hurt");
                hpBar.UpdateValue(HP);
                Camera.Main.Shake(1.5f);
            }
        }

        public void Heal(float health)
        {
            HP += health;
            HP = MathHelper.Clamp(HP, 0f, MaxHP); // Clamp HP
            hpBar.UpdateValue(HP);
        }


        public override void Initialize()
        {
            CollisionManager.Add(this, "Player"); // Add player into the collision manager

            // Define the keys and animations
            animationManager.AddAnimation(PlayerAction.Idle, new Animation(ResourcesManager.FindTexture("Player_SpriteSheet"), 2, 32, 32, idleAnimationSpeed));
            animationManager.AddAnimation(PlayerAction.Run, new Animation(ResourcesManager.FindTexture("Player_SpriteSheet"), 8, 32, 32, runAnimationSpeed, 4));
            animationManager.AddAnimation(PlayerAction.Walk, new Animation(ResourcesManager.FindTexture("Player_SpriteSheet"), 4, 32, 32, walkAnimationSpeed, 3));
            animationManager.AddAnimation(PlayerAction.Death, new Animation(ResourcesManager.FindTexture("Player_SpriteSheet"), 8, 32, 32, animationSpeed, 8, looping: false));
            animationManager.AddAnimation(PlayerAction.Dash, new Animation(ResourcesManager.FindTexture("Player_SpriteSheet"), 3, 32, 32, dashAnimationSpeed, 7, looping: false));

            // Footstep sounds sample
            footstepsSounds = new List<SoundEffect>()
            {
                ResourcesManager.FindSoundEffect("Footstep1"),
                ResourcesManager.FindSoundEffect("Footstep2"),
                ResourcesManager.FindSoundEffect("Footstep3"),
            };

            // Shadow effect initialize
            shadowEffect = new TextureEffect(ResourcesManager.FindTexture("Shadow"),
                                            new Rectangle(0, 0, 64, 64), // Crop the shadow sprite
                                            this,
                                            new Vector2(32f), new Vector2(0.5f), new Vector2(0f, -3.5f));

            // Shader initialize
            colorOverlay = ResourcesManager.FindEffect("Color_Overlay");
            colorOverlay.Parameters["overlayColor"].SetValue(Color.White.ToVector4());

            // The HP Bar of the player
            //hpBar = new ProgressBar(ResourcesManager.FindTexture("HP_Background"), ResourcesManager.FindTexture("HP_Foreground"), 100, new Vector2(25f), new Vector2(4f));
            hpBar = new ProgressBar(ResourcesManager.FindTexture("health_bar_decoration"), ResourcesManager.FindTexture("health_bar"), 100, new Vector2(25f, 10f), new Vector2(5f));
            hpBar.SetForegroundOffset(new Vector2(14f * 5, 0f));
            GameManager.AddGameObjectUI(hpBar);
            hpBar.UpdateValue(HP);

            // Calculate reloading text offset
            float x = ResourcesManager.FindSpriteFont("Font_Normal").MeasureString("Reloading 0.00s").X;
            textPosOffset = new(-x / 20f, -16f);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // Player death logic
            //if (InputManager.GetKey(Keys.K)) currentAction = PlayerAction.Death;
            if(HP <= 0) currentAction = PlayerAction.Death;
            animationManager.Update(currentAction, gameTime); // Update the animations
            shadowEffect.Update(gameTime); // Shadow of the player
            base.Update(gameTime);
            if (currentAction == PlayerAction.Death && animationManager.GetAnimation(PlayerAction.Death).Finish) IsGameOver = true; // Declare the game is over
            if (currentAction == PlayerAction.Death) return; // No need to update player logic anymore is the player is death

            if (GameManager.CurrentLevel.TouchingDoor(Bound)) GameManager.UpdateLevel(); // Update level
            Dash();
            Invincible();
            if (!dashing)
                PlayerMovement();
            Gun.Update(gameTime); // Gun logic
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            shadowEffect.Draw(spriteBatch, gameTime); // Shadow always behind the player
            spriteBatch.End(); // End previous drawing session first inorder to start new one

            // Start new drawing session with shader (Everything between this draw call will be affect by the shader apply)
            if (invincible && blink && !IsGameOver)
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, transformMatrix: Camera.Main.Transform, effect: colorOverlay);
            else
                spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, transformMatrix: Camera.Main.Transform, effect: null);

            // Draw the gun and player
            if (Gun.RenderInfront)
            {
                DrawAnimation(animationManager.CurrentAnimation, spriteBatch, gameTime);
                Gun.Draw(spriteBatch, gameTime);
            }
            else
            {
                Gun.Draw(spriteBatch, gameTime);
                DrawAnimation(animationManager.CurrentAnimation, spriteBatch, gameTime);
            }

            spriteBatch.End(); // End current drawing session
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, transformMatrix: Camera.Main.Transform); // Resume back to normal drawing session

            // Reloading Text
            if (Gun.Reloading)
            {
                spriteBatch.DrawString(ResourcesManager.FindSpriteFont("Font_Normal"), "Reloading " + Gun.Magazine.CD.ToString("N2") + "s", position + textPosOffset, reloadingStringColor, 0f, textOffset, 0.15f, SpriteEffects.None, 0f);
            }
        }
        //Color reloadingStringColor = new Color(7, 24, 33);
        Color reloadingStringColor = Color.DarkRed;

        //public override void CollisionEvent(GameObject gameObject)
        //{

        //}
    }
}
