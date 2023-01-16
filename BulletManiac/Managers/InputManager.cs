using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

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
        /// <summary>
        /// One shot click of the left mouse button
        /// </summary>
        public static bool MouseLeftClick { get; private set; }
        /// <summary>
        /// One shot click of the right mouse button
        /// </summary>
        public static bool MouseRightClick { get; private set; }
        /// <summary>
        /// Holding click of the left mouse button
        /// </summary>
        public static bool MouseLeftHold { get; private set; }
        /// <summary>
        /// Holding click of the right mouse button
        /// </summary>
        public static bool MouseRightHold { get; private set; }
        /// <summary>
        /// Release of the left mouse button
        /// </summary>
        public static bool MouseLeftReleased { get; private set; }
        /// <summary>
        /// Release of the right mouse button
        /// </summary>
        public static bool MouseRightReleased { get; private set; }


        public static bool MouseScrollUp { get; private set; }
        public static bool MouseScrollDown { get; private set; }
        private static float currentMouseWheel, previousMouseWheel;

        // Array of keys, used to check if certain key is press once
        private static int[] keyCodes;
        private static bool[] keyPress;
        private static bool[] keyDown;
        public static void Initialize()
        {
            // Initialize the key from the Keys enum
            keyCodes = Enum.GetValues(typeof(Keys)).Cast<int>().ToArray();
            keyPress = new bool[keyCodes.Length];
            keyDown = new bool[keyCodes.Length];
        }

        public static void Update(GameTime gameTime)
        {
            // Direction update based on the kaybord events
            direction = Vector2.Zero;

            currentKeyboardState = Keyboard.GetState();
            if(currentKeyboardState.GetPressedKeyCount() > 0)
            {
                // Increment / decrement based on the input
                if (currentKeyboardState.IsKeyDown(Keys.A)) direction.X--;
                if (currentKeyboardState.IsKeyDown(Keys.D)) direction.X++;
                if (currentKeyboardState.IsKeyDown(Keys.W)) direction.Y--;
                if (currentKeyboardState.IsKeyDown(Keys.S)) direction.Y++;
            }

            // Update Keys
            for (int i = 0; i < keyCodes.Length; i++)
            {
                // Key Press
                if (currentKeyboardState.IsKeyDown((Keys)keyCodes[i]) && lastKeyboardState.IsKeyUp((Keys)keyCodes[i]))
                {
                    keyPress[i] = true;
                }
                else
                {
                    keyPress[i] = false;
                }

                // Key Press Down
                if (currentKeyboardState.IsKeyDown((Keys)keyCodes[i]))
                {
                    keyDown[i] = true;
                }
                else
                {
                    keyDown[i] = false;
                }
            }

            lastKeyboardState = currentKeyboardState;

            // Mouse Update
            MouseState mouseState = Mouse.GetState();
            MousePosition = mouseState.Position.ToVector2(); // Position Update

            // Mouse Button Update
            MouseLeftClick = mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released; // The boolean will only register once
            MouseRightClick = mouseState.RightButton == ButtonState.Pressed && lastMouseState.RightButton == ButtonState.Released;
            MouseLeftHold = mouseState.LeftButton == ButtonState.Pressed;
            MouseRightHold = mouseState.RightButton == ButtonState.Pressed;
            MouseLeftReleased = mouseState.LeftButton == ButtonState.Released;
            MouseRightReleased = mouseState.RightButton == ButtonState.Released;

            previousMouseWheel = currentMouseWheel;
            currentMouseWheel = mouseState.ScrollWheelValue;

            // Mouse Scroll Update
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
            int index = Array.FindIndex(keyCodes, x => x == (int)key); // Get the index based on the keycode given
            return keyPress[index]; // Get if the key is press
        }

        public static bool GetKeyDown(Keys key)
        {
            int index = Array.FindIndex(keyCodes, x => x == (int)key); // Get the index based on the keycode given
            return keyDown[index]; // Get if the key is press
        }
    }
}
