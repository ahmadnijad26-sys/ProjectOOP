// Items/RelicItem.cs
using Microsoft.Xna.Framework;

namespace ProjectOOPGame_Fresh
{
    public class RelicItem
    {
        public string Name;
        public Color DisplayColor;
        public int DamagePercent;

        public RelicItem(string name, Color color, int damagePercent)
        {
            Name = name;
            DisplayColor = color;
            DamagePercent = damagePercent;
        }
    }
}