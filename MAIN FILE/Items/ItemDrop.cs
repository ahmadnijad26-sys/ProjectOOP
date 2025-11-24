// Items/ItemDrop.cs
using Microsoft.Xna.Framework;

namespace ProjectOOPGame_Fresh
{
    public class ItemDrop
    {
        public Vector2 Position;
        public string ItemName;
        public Rectangle Bounds;
        public Color DisplayColor;

        public ItemDrop(Vector2 pos, string itemName)
        {
            Position = pos;
            ItemName = itemName;
            Bounds = new Rectangle((int)pos.X, (int)pos.Y, 24, 24);
            DisplayColor = itemName == "Key" ? Color.Gold :
                          itemName == "Spiritvine Blade" ? Color.Cyan : Color.White;
        }
    }
}