using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectOOPGame_Fresh
{
    public interface IGameLevel
    {
        Player Player { get; }
        bool IsCompleted { get; }
        void Initialize();
        void Update(float deltaTime, KeyboardState keyState, KeyboardState prevKeyState);
        void Draw(SpriteBatch spriteBatch, Camera camera);
    }
}