﻿using BulletManiac.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BulletManiac.Entity.Player
{
    public class Player : GameObject
    {
        AnimationManager animationManager;
        float moveSpeed = 300f;
        float animationSpeed = 0.08f;
        public Player()
        {
            position = new Vector2(100f, 100f);
            animationManager = new AnimationManager();

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
            return Rectangle.Empty;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // Player movement
            if (InputManager.Moving)
            {
                position += Vector2.Normalize(InputManager.Direction) * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            // Update the animations
            animationManager.Update(InputManager.Direction, gameTime);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            animationManager.CurrentAnimation.Draw(spriteBatch, position, Color.White, 0f, origin, new Vector2(5f, 5f), SpriteEffects.None, 0f);
        }
    }
}
