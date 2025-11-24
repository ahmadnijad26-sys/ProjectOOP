// Objects/InteractiveObject.cs
using Microsoft.Xna.Framework;

namespace ProjectOOPGame_Fresh
{
    public class InteractiveObject
    {
        public Vector2 Position;
        public Rectangle Bounds;
        public string Type;
        public bool IsActivated;

        public InteractiveObject(Vector2 pos, string type, int width = 48, int height = 48)
        {
            Position = pos;
            Type = type;
            Bounds = new Rectangle((int)pos.X, (int)pos.Y, width, height);
            IsActivated = false;
        }
    }
}