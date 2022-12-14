using BulletManiac.Collision;
using BulletManiac.Managers;
using BulletManiac.Utilities;
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
        enum PlayerAction
        {
            Idle, Run, Walk, Death, Throw
        }

        private AnimationManager animationManager; // Manange the animation based on certain action

        float moveSpeed = 100f;
        float animationSpeed = 0.1f;
        float runAnimationSpeed = 0.1f;
        float attackAnimationSpeed = 0.1f;
        float shootSpeed = 1f;

        private bool shooting = false;

        PlayerAction currentAction = PlayerAction.Idle;
        public Player(Vector2 position)
        {
            name = "Player";
            this.position = position;
            animationManager = new AnimationManager();
            scale = new Vector2(0.7f); // Scale of the player
            origin = new Vector2(16f); // Origin (Half of the sprite size)
        }

        protected override Rectangle CalculateBound()
        {
            if(spriteEffects == SpriteEffects.None)
            {
                Vector2 pos = position - (origin * scale / 1.1f) + new Vector2(2f, 0f); ;
                return new Rectangle((int)pos.X, (int)pos.Y, (int)(texture.Width * scale.X / 1.25f), (int)(texture.Height * scale.Y));
            }
            else
            {
                Vector2 pos = position - (origin * scale / 1.1f) + new Vector2(2f, 0f);
                return new Rectangle((int)pos.X, (int)pos.Y, (int)(texture.Width * scale.X / 1.25f), (int)(texture.Height * scale.Y));
            }
        }

        public override void Initialize()
        {
            CollisionManager.Add(this, Position.ToString()); // Testing Collision

            // Load player sprites
            GameManager.Resources.LoadTexture("Player_Death", "SpriteSheet/Player/Owlet_Monster_Death_8");
            GameManager.Resources.LoadTexture("Player_Idle", "SpriteSheet/Player/Owlet_Monster_Idle_4");
            GameManager.Resources.LoadTexture("Player_Walk", "SpriteSheet/Player/Owlet_Monster_Walk_6");
            GameManager.Resources.LoadTexture("Player_Run", "SpriteSheet/Player/Owlet_Monster_Run_6");
            GameManager.Resources.LoadTexture("Player_Throw", "SpriteSheet/Player/Owlet_Monster_Throw_4");


            // Define the keys and animations
            animationManager.AddAnimation(PlayerAction.Idle, new Animation(GameManager.Resources.FindTexture("Player_Idle"), 4, 1, animationSpeed));
            animationManager.AddAnimation(PlayerAction.Run, new Animation(GameManager.Resources.FindTexture("Player_Run"), 6, 1, runAnimationSpeed));
            animationManager.AddAnimation(PlayerAction.Walk, new Animation(GameManager.Resources.FindTexture("Player_Walk"), 6, 1, animationSpeed));
            animationManager.AddAnimation(PlayerAction.Death, new Animation(GameManager.Resources.FindTexture("Player_Death"), 8, 1, animationSpeed));
            animationManager.AddAnimation(PlayerAction.Throw, new Animation(GameManager.Resources.FindTexture("Player_Throw"), 4, 1, attackAnimationSpeed * shootSpeed, looping: false));

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
                shooting = true;
                currentAction = PlayerAction.Throw;
                GameManager.MainCamera.Shake();

                // Spawn Bullet
                Vector2 mousePos = Camera.ScreenToWorld(InputManager.MousePosition); // Convert mouse screen position to the world position
                Vector2 bulletDirection = mousePos - position;

                Bullet bullet = new Bullet(position, bulletDirection, 100f);
                GameManager.AddGameObject(bullet); // Straight away add bullet to entity manager to run it
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
                if (InputManager.GetKeyDown(Keys.LeftShift))
                {
                    position += Vector2.Normalize(InputManager.Direction) * (50f) * GameManager.DeltaTime;
                    currentAction = PlayerAction.Walk;
                }
                else
                {
                    position += Vector2.Normalize(InputManager.Direction) * moveSpeed * GameManager.DeltaTime;
                    currentAction = PlayerAction.Run;
                }

                // Set animation reverse
                if(spriteEffects == SpriteEffects.None && InputManager.Direction.X < 0 || 
                   spriteEffects == SpriteEffects.FlipHorizontally && InputManager.Direction.X >= 0)
                {
                    animationManager.GetAnimation(PlayerAction.Run).SetReverse(true);
                }
                else
                {
                    animationManager.GetAnimation(PlayerAction.Run).SetReverse(false);
                }
            }
            else
            {
                currentAction = PlayerAction.Idle; // Player is Idle when not moving
            }
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
