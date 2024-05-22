using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Collections.Generic;


namespace Tutorial
{
    public class Input
    {
        private static KeyboardState currentState;
        private static KeyboardState previousState;
        public static KeyboardState Update()
        {
            previousState = currentState;
            currentState = Keyboard.GetState();
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
    }
}
