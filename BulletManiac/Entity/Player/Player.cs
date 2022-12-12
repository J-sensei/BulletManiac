using BulletManiac.Collision;
using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace BulletManiac.Entity.Player
{
    public class Player : GameObject
    {
        AnimationManager animationManager;

        float moveSpeed = 100f;
        float animationSpeed = 0.08f;

        public bool move = true; // test
        public Player(Vector2 position)
        {
            name = "Player";
            this.position = position;
            animationManager = new AnimationManager();
            scale = new Vector2(1f);
            origin = new Vector2(16f, 16f);

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
            float x = 1.75f;
            Vector2 pos = position - origin / x;
            return new Rectangle((int)pos.X, (int)pos.Y, (int)(texture.Width / x), (int)(texture.Height / x));
        }

        public override void Initialize()
        {
            CollisionManager.Add(this, Position.ToString()); // Testing Collision
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //Console.WriteLine(Position);
            // Player movement
            if (InputManager.Moving && move)
            {
                position += Vector2.Normalize(InputManager.Direction) * moveSpeed * GameManager.DeltaTime;
            }

            // Update the animations
            animationManager.Update(InputManager.Direction, gameTime);
            texture = animationManager.CurrentAnimation.CurrentTexture; // Update the texture based on the animation
            base.Update(gameTime);
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
