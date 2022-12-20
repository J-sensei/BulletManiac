using BulletManiac.Collision;
using BulletManiac.Managers;
using BulletManiac.Utilities;
using BulletManiac.Entity.Bullet;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace BulletManiac.Entity.Player
{
    public class Player : GameObject
    {
        #region Preserve Code

        //private AnimationManager animationManager; // Manange the animation based on certain action
        //private Vector2 cursorDirection;

        //float moveSpeed = 100f;
        //float animationSpeed = 0.08f;

        //public bool move = true; // test
        //public Player(Vector2 position)
        //{
        //    name = "Player";
        //    this.position = position;
        //    animationManager = new AnimationManager();
        //    scale = new Vector2(1f); // Scale of the player
        //    origin = new Vector2(16f); // Origin (Half of the sprite size)

        //    // Load player sprites
        //    GameManager.Resources.LoadTexture("Player_Down", "Test/TopDownCharacter/Character_Down");
        //    GameManager.Resources.LoadTexture("Player_DownLeft", "Test/TopDownCharacter/Character_DownLeft");
        //    GameManager.Resources.LoadTexture("Player_DownRight", "Test/TopDownCharacter/Character_DownRight");
        //    GameManager.Resources.LoadTexture("Player_Left", "Test/TopDownCharacter/Character_Left");
        //    GameManager.Resources.LoadTexture("Player_Right", "Test/TopDownCharacter/Character_Right");
        //    GameManager.Resources.LoadTexture("Player_Up", "Test/TopDownCharacter/Character_Up");
        //    GameManager.Resources.LoadTexture("Player_UpLeft", "Test/TopDownCharacter/Character_UpLeft");
        //    GameManager.Resources.LoadTexture("Player_UpRight", "Test/TopDownCharacter/Character_UpRight");

        //    // Define the keys and animations
        //    animationManager.AddAnimation(new Vector2(0, 0), new Animation(GameManager.Resources.FindTexture("Player_Down"), 4, 1, animationSpeed));
        //    animationManager.AddAnimation(new Vector2(0, 1), new Animation(GameManager.Resources.FindTexture("Player_Down"), 4, 1, animationSpeed));
        //    animationManager.AddAnimation(new Vector2(-1, 0), new Animation(GameManager.Resources.FindTexture("Player_Left"), 4, 1, animationSpeed));
        //    animationManager.AddAnimation(new Vector2(1, 0), new Animation(GameManager.Resources.FindTexture("Player_Right"), 4, 1, animationSpeed));
        //    animationManager.AddAnimation(new Vector2(0, -1), new Animation(GameManager.Resources.FindTexture("Player_Up"), 4, 1, animationSpeed));
        //    animationManager.AddAnimation(new Vector2(-1, 1), new Animation(GameManager.Resources.FindTexture("Player_DownLeft"), 4, 1, animationSpeed));
        //    animationManager.AddAnimation(new Vector2(-1, -1), new Animation(GameManager.Resources.FindTexture("Player_UpLeft"), 4, 1, animationSpeed));
        //    animationManager.AddAnimation(new Vector2(1, 1), new Animation(GameManager.Resources.FindTexture("Player_DownRight"), 4, 1, animationSpeed));
        //    animationManager.AddAnimation(new Vector2(1, -1), new Animation(GameManager.Resources.FindTexture("Player_UpRight"), 4, 1, animationSpeed));
        //}

        //protected override Rectangle CalculateBound()
        //{
        //    //float x = 1.75f;
        //    //Vector2 pos = position - origin / x;
        //    //return new Rectangle((int)pos.X, (int)pos.Y, (int)(texture.Width / x), (int)(texture.Height / x));

        //    Vector2 pos = position - (origin * scale / 2f);
        //    return new Rectangle((int)pos.X, (int)pos.Y, (int)(texture.Width * scale.X / 2f), (int)(texture.Height * scale.Y / 2f));
        //}

        //public override void Initialize()
        //{
        //    CollisionManager.Add(this, Position.ToString()); // Testing Collision
        //    base.Initialize();
        //}

        //public override void Update(GameTime gameTime)
        //{
        //    //Console.WriteLine(Position);
        //    // Player movement
        //    if (InputManager.Moving && move)
        //    {
        //        position += Vector2.Normalize(InputManager.Direction) * moveSpeed * GameManager.DeltaTime;
        //        animationManager.Start();
        //    }
        //    else
        //    {
        //        animationManager.Stop();
        //    }

        //    UpdateCursorDirection();

        //    // Update the animations
        //    animationManager.Update(cursorDirection, gameTime);
        //    texture = animationManager.CurrentAnimation.CurrentTexture; // Update the texture based on the animation
        //    base.Update(gameTime);
        //}

        //public void UpdateCursorDirection()
        //{
        //    Vector2 halfScreen = GameManager.CurrentResolution.ToVector2() / 2f;
        //    Vector2 mousePos = InputManager.MousePosition;
        //    cursorDirection = Vector2.Zero;
        //    float offset = 25f * GameManager.CurrentCameraZoom;
        //    if(mousePos.X < halfScreen.X - offset)
        //    {
        //        cursorDirection.X = -1;
        //    }
        //    else if(mousePos.X > halfScreen.X + offset)
        //    {
        //        cursorDirection.X = 1;
        //    }

        //    if (mousePos.Y < halfScreen.Y - offset / 2f)
        //    {
        //        cursorDirection.Y = -1;
        //    }
        //    else if (mousePos.Y > halfScreen.Y + offset / 2f)
        //    {
        //        cursorDirection.Y = 1;
        //    }
        //    // Console.WriteLine(halfScreen + " | " + mousePos + " | " + cursorDirection);
        //    //Console.WriteLine(mousePos + " | " + cursorDirection);
        //}

        //public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        //{
        //    base.Draw(spriteBatch, gameTime);
        //    //animationManager.CurrentAnimation.Draw(spriteBatch, position, Color.White, 0f, origin, new Vector2(3f, 3f), SpriteEffects.None, 0f);
        //}

        //public override void CollisionEvent(GameObject gameObject)
        //{

        //}
        #endregion
        /// <summary>
        /// Player actions, used to determine animation
        /// </summary>
        enum PlayerAction
        {
            Idle, Run, Walk, Death, Throw
        }

        private AnimationManager animationManager; // Manange the animation based on certain action

        // Player status
        float moveSpeed = 80f;
        float animationSpeed = 0.1f;
        float runAnimationSpeed = 0.1f;
        float attackAnimationSpeed = 0.065f; // Attach speed
        float shootSpeed = 1f;

        // Accuracy
        const float DEFAULT_ACCURACY = 0.25f;
        float accuracy = DEFAULT_ACCURACY; // The lower the number, the better accuracy

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
            animationManager.AddAnimation(PlayerAction.Idle, new Animation(GameManager.Resources.FindTexture("Player_Idle"), 4, 1, animationSpeed));
            animationManager.AddAnimation(PlayerAction.Run, new Animation(GameManager.Resources.FindTexture("Player_Run"), 6, 1, runAnimationSpeed));
            animationManager.AddAnimation(PlayerAction.Walk, new Animation(GameManager.Resources.FindTexture("Player_Walk"), 6, 1, animationSpeed));
            animationManager.AddAnimation(PlayerAction.Death, new Animation(GameManager.Resources.FindTexture("Player_Death"), 8, 1, animationSpeed));
            animationManager.AddAnimation(PlayerAction.Throw, new Animation(GameManager.Resources.FindTexture("Player_Throw"), 4, 1, attackAnimationSpeed * shootSpeed, looping: false));
        }

        protected override Rectangle CalculateBound()
        {
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

        public override void Initialize()
        {
            CollisionManager.Add(this, Position.ToString()); // Add player into the collision manager

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            PlayerMovement();
            PlayerAttack();

            // Update the animations
            animationManager.Update(currentAction, gameTime);
            texture = animationManager.CurrentAnimation.CurrentTexture; // Update the texture based on the animation

            base.Update(gameTime);
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
                }
                else
                {
                    animationManager.GetAnimation(PlayerAction.Run).SetReverse(false);
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

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            base.Draw(spriteBatch, gameTime);
            //animationManager.CurrentAnimation.Draw(spriteBatch, position, Color.White, 0f, origin, new Vector2(3f, 3f), SpriteEffects.None, 0f);
        }

        public override void CollisionEvent(GameObject gameObject)
        {

        }
    }
}
