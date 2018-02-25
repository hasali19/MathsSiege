using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MathsSiege.Client.Framework
{
    public enum MouseButton
    {
        Left,
        Right,
        Middle
    }

    public static class InputHandler
    {
        public static MouseState MouseState { get; private set; }
        public static KeyboardState KeyboardState { get; private set; }

        public static MouseState PrevMouseState { get; private set; }
        public static KeyboardState PrevKeyboardState { get; private set; }

        public static Vector2 MousePosition { get; private set; }
        public static int MouseScrollValue => MouseState.ScrollWheelValue;
        public static int MouseScrollOffset { get; private set; }

        public static void Update()
        {
            PrevMouseState = MouseState;
            PrevKeyboardState = KeyboardState;

            MouseState = Mouse.GetState();
            KeyboardState = Keyboard.GetState();

            MousePosition = MouseState.Position.ToVector2();
            MouseScrollOffset = MouseState.ScrollWheelValue - PrevMouseState.ScrollWheelValue;
        }

        /// <summary>
        /// Checks whether the mouse button is currently pressed.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool IsMouseButtonDown(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return MouseState.LeftButton == ButtonState.Pressed;

                case MouseButton.Right:
                    return MouseState.RightButton == ButtonState.Pressed;

                case MouseButton.Middle:
                    return MouseState.MiddleButton == ButtonState.Pressed;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks whether the mouse button is currently released.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool IsMouseButtonUp(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return MouseState.LeftButton == ButtonState.Released;

                case MouseButton.Right:
                    return MouseState.RightButton == ButtonState.Released;

                case MouseButton.Middle:
                    return MouseState.MiddleButton == ButtonState.Released;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks whether the mouse button is pressed this tick but not the last.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool IsMouseButtonPressed(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return MouseState.LeftButton == ButtonState.Pressed && PrevMouseState.LeftButton == ButtonState.Released;

                case MouseButton.Right:
                    return MouseState.RightButton == ButtonState.Pressed && PrevMouseState.RightButton == ButtonState.Released;

                case MouseButton.Middle:
                    return MouseState.MiddleButton == ButtonState.Pressed && PrevMouseState.MiddleButton == ButtonState.Released;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if the mouse button is released this tick but not the last.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static bool IsMouseButtonReleased(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return MouseState.LeftButton == ButtonState.Released && PrevMouseState.LeftButton == ButtonState.Pressed;

                case MouseButton.Right:
                    return MouseState.RightButton == ButtonState.Released && PrevMouseState.RightButton == ButtonState.Pressed;

                case MouseButton.Middle:
                    return MouseState.MiddleButton == ButtonState.Released && PrevMouseState.MiddleButton == ButtonState.Pressed;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if the key is currently pressed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyDown(Keys key)
        {
            return KeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Checks if the key is currently released.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyUp(Keys key)
        {
            return KeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if the key is pressed this tick but not the last.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyPressed(Keys key)
        {
            return KeyboardState.IsKeyDown(key) && PrevKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Checks if the key is released this tick but not the last.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool IsKeyReleased(Keys key)
        {
            return KeyboardState.IsKeyUp(key) && PrevKeyboardState.IsKeyDown(key);
        }
    }
}
