// Systems/InputManager.cs
using Microsoft.Xna.Framework.Input;

namespace ProjectOOPGame_Fresh
{
    public class InputManager
    {
        public KeyboardState CurrentState { get; private set; }
        public KeyboardState PreviousState { get; private set; }

        public void Update()
        {
            PreviousState = CurrentState;
            CurrentState = Keyboard.GetState();
        }

        public bool IsKeyPressed(Keys key)
        {
            return CurrentState.IsKeyDown(key) && PreviousState.IsKeyUp(key);
        }

        public bool IsKeyDown(Keys key)
        {
            return CurrentState.IsKeyDown(key);
        }
    }
}