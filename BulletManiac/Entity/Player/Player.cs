using BulletManiac.Collision;
using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Entity.Player
{
    public class Player : GameObject
    {

        private AnimationManager animationManager; // Manange the animation based on certain action
        private Vector2 cursorDirection;

        float moveSpeed = 100f;
        float animationSpeed = 0.08f;

        private Gun gun;

        public bool move = true; // test
        public Player(Vector2 position)
        {
            name = "Player";
            this.position = position;
            animationManager = new AnimationManager();
            scale = new Vector2(1f); // Scale of the player
            origin = new Vector2(16f); // Origin (Half of the sprite size)

            gun = new Gun();

            // Load player sprites
            GameManager.Resources.LoadTexture("Player_Down", "Test/TopDownCharacter/Character_Down");
            GameManager.Resources.LoadTexture("Player_DownLeft", "Test/TopDownCharacter/Character_DownLeft");
            GameManager.Resources.LoadTexture("Player_DownRight", "Test/TopDownCharacter/Character_DownRight");
            GameManager.Resources.LoadTexture("Player_Left", "Test/TopDownCharacter/Character_Left");
            GameManager.Resources.LoadTexture("Player_Right", "Test/TopDownCharacter/Character_Right");
            GameManager.Resources.LoadTexture("Player_Up", "Test/TopDownCharacter/Character_Up");
            GameManager.Resources.LoadTexture("Player_UpLeft", "Test/TopDownCharacter/Character_UpLeft");
            GameManager.Resources.LoadTexture("Player_UpRight", "Test/TopDownCharacter/Character_UpRight");

            // Define the keys and animations
            animationManager.AddAnimation(new Vector2(0, 0), new Animation(GameManager.Resources.FindTexture("Player_Down"), 4, 1, animationSpeed));
            animationManager.AddAnimation(new Vector2(0, 1), new Animation(GameManager.Resources.FindTexture("Player_Down"), 4, 1, animationSpeed));
            animationManager.AddAnimation(new Vector2(-1, 0), new Animation(GameManager.Resources.FindTexture("Player_Left"), 4, 1, animationSpeed));
            animationManager.AddAnimation(new Vector2(1, 0), new Animation(GameManager.Resources.FindTexture("Player_Right"), 4, 1, animationSpeed));
            animationManager.AddAnimation(new Vector2(0, -1), new Animation(GameManager.Resources.FindTexture("Player_Up"), 4, 1, animationSpeed));
            animationManager.AddAnimation(new Vector2(-1, 1), new Animation(GameManager.Resources.FindTexture("Player_DownLeft"), 4, 1, animationSpeed));
            animationManager.AddAnimation(new Vector2(-1, -1), new Animation(GameManager.Resources.FindTexture("Player_UpLeft"), 4, 1, animationSpeed));
            animationManager.AddAnimation(new Vector2(1, 1), new Animation(GameManager.Resources.FindTexture("Player_DownRight"), 4, 1, animationSpeed));
            animationManager.AddAnimation(new Vector2(1, -1), new Animation(GameManager.Resources.FindTexture("Player_UpRight"), 4, 1, animationSpeed));
        }

        protected override Rectangle CalculateBound()
        {
            //float x = 1.75f;
            //Vector2 pos = position - origin / x;
            //return new Rectangle((int)pos.X, (int)pos.Y, (int)(texture.Width / x), (int)(texture.Height / x));

            Vector2 pos = position - (origin * scale / 2f);
            return new Rectangle((int)pos.X, (int)pos.Y, (int)(texture.Width * scale.X / 2f), (int)(texture.Height * scale.Y / 2f));
        }

        public override void Initialize()
        {
            CollisionManager.Add(this, Position.ToString()); // Testing Collision
            base.Initialize();
            gun.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //Console.WriteLine(Position);
            // Player movement
            if (InputManager.Moving && move)
            {
                position += Vector2.Normalize(InputManager.Direction) * moveSpeed * GameManager.DeltaTime;
                animationManager.Start();
            }
            else
            {
                animationManager.Stop();
            }

            UpdateCursorDirection();

            gun.Follow(this, new Vector2(0f, 0f));
            // Update the animations
            animationManager.Update(cursorDirection, gameTime);
            texture = animationManager.CurrentAnimation.CurrentTexture; // Update the texture based on the animation
            base.Update(gameTime);
            gun.Update(gameTime);
        }

        public void UpdateCursorDirection()
        {
            Vector2 halfScreen = GameManager.CurrentResolution.ToVector2() / 2f;
            Vector2 mousePos = InputManager.MousePosition;
            cursorDirection = Vector2.Zero;
            float offset = 25f * GameManager.CurrentCameraZoom;
            if(mousePos.X < halfScreen.X - offset)
            {
                cursorDirection.X = -1;
            }
            else if(mousePos.X > halfScreen.X + offset)
            {
                cursorDirection.X = 1;
            }

            if (mousePos.Y < halfScreen.Y - offset / 2f)
            {
                cursorDirection.Y = -1;
            }
            else if (mousePos.Y > halfScreen.Y + offset / 2f)
            {
                cursorDirection.Y = 1;
            }
            // Console.WriteLine(halfScreen + " | " + mousePos + " | " + cursorDirection);
            //Console.WriteLine(mousePos + " | " + cursorDirection);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            gun.Draw(spriteBatch, gameTime);
            base.Draw(spriteBatch, gameTime);
            //animationManager.CurrentAnimation.Draw(spriteBatch, position, Color.White, 0f, origin, new Vector2(3f, 3f), SpriteEffects.None, 0f);
        }

        public override void CollisionEvent(GameObject gameObject)
        {
            
        }
    }
}
