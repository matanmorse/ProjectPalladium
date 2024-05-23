using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using ProjectPalladium.Utils;
using System.Diagnostics;

namespace Tutorial
{
    public class Input
    {
        public static Point previousMousePos;
        public static Point mousePos;

        private static KeyboardState currentState;
        private static KeyboardState previousState;

        private static MouseState currentMouseState;
        private static MouseState previousMouseState;

        public static KeyboardState Update()
        {
            mousePos = Util.GetNativeMousePos(GetMousePos());

            previousState = currentState;
            currentState = Keyboard.GetState();

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            return currentState;
        }

        public static Vector2 GetMovement()
        {
            return new Vector2(GetHorizontal(), GetVertical());
        }

        public static float GetHorizontal()
        {
            return (Keyboard.GetState().IsKeyDown(Keys.A) ? -1 : 0) + (Keyboard.GetState().IsKeyDown(Keys.D) ? 1 : 0);
        }
        public static float GetVertical()
        {
            return (Keyboard.GetState().IsKeyDown(Keys.W) ? -1 : 0) + (Keyboard.GetState().IsKeyDown(Keys.S) ? 1 : 0);
        }
        public static bool GetKey(Keys k) 
        { 
            return Keyboard.GetState().IsKeyDown(k);
        }
        
        public static bool GetKeyDown(Keys k)
        {
            return currentState.IsKeyDown(k) && !previousState.IsKeyDown(k);
        }

        public static Point GetMousePos()
        {
            previousMousePos = mousePos;
            return currentMouseState.Position;
        }
        public static bool GetLeftMouseDown()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed;
        }
    }
}
