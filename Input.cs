using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using ProjectPalladium.Utils;
using System.Diagnostics;

namespace ProjectPalladium
{
    public class Input
    {
        public static Point previousNativeMousePos;
        public static Point nativeMousePos;

        public static Point gameWorldMousePos;
        public static Point prevGameWorldMousePos;

        private static KeyboardState currentState;
        private static KeyboardState previousState;

        public static MouseState currentMouseState;
        private static MouseState previousMouseState;

        public static KeyboardState Update()
        {
            Point rawMousePos = GetMousePos();

            nativeMousePos = Util.GetNativeMousePos(rawMousePos);

            gameWorldMousePos = Util.GetGameworldMousePos(rawMousePos);

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
            prevGameWorldMousePos = gameWorldMousePos;
            previousNativeMousePos = nativeMousePos;
            return currentMouseState.Position;
        }
        public static bool GetLeftMouseClicked()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton != ButtonState.Pressed;
        }

        public static bool GetRightMouseClicked()
        {
            return currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton != ButtonState.Pressed;
        }


        public static bool GetLeftMouseDown()
        {
            return currentMouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool GetRightMouseDown()
        {
            return currentMouseState.RightButton == ButtonState.Pressed;
        }



    }
}
