using Microsoft.Xna.Framework;

namespace ProjectOOPGame_Fresh
{
    public class TutoLevel : Level
    {
        public TutoLevel(Player player) : base(player)
        {
        }

        protected override void InitializeMap()
        {
            Map = new TileType[mapWidth, mapHeight];

            // Initialize your map tiles here (similar to your original InitializeMap)
            for (int x = 0; x < mapWidth; x++)
                for (int y = 0; y < mapHeight; y++)
                    Map[x, y] = TileType.Grass;

            // Add walls, water, etc.
            for (int x = 0; x < mapWidth; x++)
            {
                Map[x, 0] = TileType.Tree;
                Map[x, mapHeight - 1] = TileType.Tree;
            }
            // ... rest of your map initialization
        }

        protected override void InitializeEnemies()
        {
            // Add enemies for this level
            Enemies.Add(new Enemy(new Vector2(tileSize * 4, tileSize * 5), EnemyType.Snake, true));
            Enemies.Add(new Enemy(new Vector2(tileSize * 8, tileSize * 4), EnemyType.Snake, true));
            // ... more enemies
        }

        protected override void InitializeObjects()
        {
            // Add interactive objects for this level
            Objects.Add(new InteractiveObject(new Vector2(tileSize * 2, tileSize * 9), "Vine", 64, 64));
            Objects.Add(new InteractiveObject(new Vector2(tileSize * 13, tileSize * 13), "Chest", 64, 64));
        }
    }
}
