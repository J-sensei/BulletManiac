using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BulletManiac.Managers
{
    /// <summary>
    /// Manage the input necessary for the game
    /// </summary>
    public static class InputManager
    {
        private static Vector2 direction;
        /// <summary>
        /// Walk direction for the player
        /// </summary>
        public static Vector2 Direction => direction;
        /// <summary>
        /// Is the player moving
        /// </summary>
        public static bool Moving => direction != Vector2.Zero;

        public static void Update(GameTime gameTime)
        {
            direction = Vector2.Zero;

            KeyboardState keyboardState = Keyboard.GetState();

            if(keyboardState.GetPressedKeyCount() > 0)
            {
                if (keyboardState.IsKeyDown(Keys.A)) direction.X--;
                if (keyboardState.IsKeyDown(Keys.D)) direction.X++;
                if (keyboardState.IsKeyDown(Keys.W)) direction.Y--;
                if (keyboardState.IsKeyDown(Keys.S)) direction.Y++;
            }
        }
    }
}
