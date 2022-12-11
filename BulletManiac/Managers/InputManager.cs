using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace BulletManiac.Managers
{
    /// <summary>
    /// Manage the input necessary for the game
    /// </summary>
    public static class InputManager
    {
        private static KeyboardState currentKeyboardState;
        private static KeyboardState lastKeyboardState;
        private static MouseState lastMouseState;
        private static Vector2 direction;
        /// <summary>
        /// Walk direction for the player
        /// </summary>
        public static Vector2 Direction => direction;
        /// <summary>
        /// Is the player moving
        /// </summary>
        public static bool Moving => direction != Vector2.Zero;
        public static Vector2 MousePosition { get; private set; }
        public static bool MouseLeftClick { get; private set; }
        public static bool MouseRightClick { get; private set; }
        public static bool MouseScrollUp { get; private set; }
        public static bool MouseScrollDown { get; private set; }
        private static float currentMouseWheel, previousMouseWheel;

        public static void Update(GameTime gameTime)
        {
            // Direction update based on the kaybord events
            direction = Vector2.Zero;

            KeyboardState keyboardState = Keyboard.GetState();
            if(keyboardState.GetPressedKeyCount() > 0)
            {
                // Increment / decrement based on the input
                if (keyboardState.IsKeyDown(Keys.A)) direction.X--;
                if (keyboardState.IsKeyDown(Keys.D)) direction.X++;
                if (keyboardState.IsKeyDown(Keys.W)) direction.Y--;
                if (keyboardState.IsKeyDown(Keys.S)) direction.Y++;
            }

            // Mouse Update
            MouseState mouseState = Mouse.GetState();
            MousePosition = mouseState.Position.ToVector2(); // Position Update
            MouseLeftClick = mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released; // The boolean will only register once
            MouseRightClick = mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released;

            previousMouseWheel = currentMouseWheel;
            currentMouseWheel = mouseState.ScrollWheelValue;

            MouseScrollUp = MouseScrollDown = false;
            if (currentMouseWheel > previousMouseWheel)
            {
                MouseScrollUp = true;
            }

            if (currentMouseWheel < previousMouseWheel)
            {
                MouseScrollDown = true;
            }

            lastMouseState = mouseState; // Store the last mouse state for the comparison
        }

        /// <summary>
        /// Get the key press once
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetKey(Keys key)
        {
            currentKeyboardState = Keyboard.GetState();
            if (currentKeyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key))
            {
                lastKeyboardState = currentKeyboardState;
                return true;
            }
            else
            {
                lastKeyboardState = currentKeyboardState;
                return false;
            }
        }
    }
}
