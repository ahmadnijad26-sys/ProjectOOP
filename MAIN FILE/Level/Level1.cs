using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectOOPGame_Fresh
{
    public class Level1 : IGameLevel
    {
        public Player Player { get; private set; }
        public bool IsCompleted { get; private set; }
        
        private List<Enemy> enemies;
        private List<InteractiveObject> objects;
        private List<ItemDrop> itemDrops;
        private TileType[,] map;
        private int mapWidth = 16, mapHeight = 16, tileSize = 64;
        private Random random;
        private int snakesKilled = 0, spidersKilled = 0;
        private bool hasRelicBlade = false, vineIsPushed = false;
        private float enemyRespawnTimer = 0f, enemyRespawnDelay = 30f;
        private Texture2D pixelTexture;

        public Level1(Player player)
        {
            Player = player;
            random = new Random();
            enemies = new List<Enemy>();
            objects = new List<InteractiveObject>();
            itemDrops = new List<ItemDrop>();
        }

        public void Initialize()
        {
            // JUNGLE THEME MAP
            map = new TileType[mapWidth, mapHeight];
            
            // Fill with jungle grass
            for (int x = 0; x < mapWidth; x++)
                for (int y = 0; y < mapHeight; y++)
                    map[x, y] = TileType.Grass;

            // Jungle borders (dense trees)
            for (int x = 0; x < mapWidth; x++)
            {
                map[x, 0] = TileType.Tree;
                map[x, mapHeight - 1] = TileType.Tree;
            }
            for (int y = 0; y < mapHeight; y++)
            {
                map[0, y] = TileType.Tree;
                map[mapWidth - 1, y] = TileType.Tree;
            }

            // Jungle river
            for (int x = 1; x < 15; x++) map[x, 10] = TileType.Water;

            // Jungle tree clusters
            map[3, 3] = TileType.Tree;
            map[5, 4] = TileType.Tree;
            map[10, 3] = TileType.Tree;
            map[12, 5] = TileType.Tree;
            map[4, 7] = TileType.Tree;
            map[11, 8] = TileType.Tree;
            map[13, 13] = TileType.Chest;
            map[14, 13] = TileType.Exit;

            // JUNGLE ENEMIES
            enemies.Clear();
            enemies.Add(new Enemy(new Vector2(tileSize * 4, tileSize * 5), EnemyType.Snake, true));
            enemies.Add(new Enemy(new Vector2(tileSize * 8, tileSize * 4), EnemyType.Snake, true));
            enemies.Add(new Enemy(new Vector2(tileSize * 6, tileSize * 8), EnemyType.Snake, true));
            enemies.Add(new Enemy(new Vector2(tileSize * 10, tileSize * 6), EnemyType.Spider, true));
            enemies.Add(new Enemy(new Vector2(tileSize * 5, tileSize * 12), EnemyType.Spider, true));
            enemies.Add(new Enemy(new Vector2(tileSize * 11, tileSize * 12), EnemyType.Spider, true));

            // JUNGLE OBJECTS
            objects.Clear();
            objects.Add(new InteractiveObject(new Vector2(tileSize * 2, tileSize * 9), "Vine", 64, 64));
            objects.Add(new InteractiveObject(new Vector2(tileSize * 13, tileSize * 13), "Chest", 64, 64));

            itemDrops.Clear();
            snakesKilled = 0;
            spidersKilled = 0;
            hasRelicBlade = false;
            vineIsPushed = false;
            IsCompleted = false;

            // Start player in jungle
            Player.Position = new Vector2(tileSize * 2, tileSize * 2);
            Player.Health = Player.MaxHealth;
            Player.State = EntityState.Idle;
        }

        public void Update(float deltaTime, KeyboardState keyState, KeyboardState prevKeyState)
        {
            if (Player.State == EntityState.Dead) return;

            Player.Update(deltaTime, keyState, prevKeyState);

            // Boundary checking
            Player.Position.X = MathHelper.Clamp(Player.Position.X, tileSize, (mapWidth - 2) * tileSize);
            Player.Position.Y = MathHelper.Clamp(Player.Position.Y, tileSize, (mapHeight - 2) * tileSize);

            CheckTileCollisions();

            foreach (var enemy in enemies)
            {
                enemy.Update(deltaTime, Player, random);
            }

            if (Player.IsAttacking) CheckPlayerAttack();
            CheckItemPickups(keyState, prevKeyState);
            CheckObjectInteractions(keyState, prevKeyState);

            enemyRespawnTimer += deltaTime;
            if (enemyRespawnTimer >= enemyRespawnDelay)
            {
                RespawnEnemies();
                enemyRespawnTimer = 0f;
            }

            if (hasRelicBlade) CheckExitReached();
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            var screenCenter = new Vector2(1280 / 2f, 720 / 2f);
            spriteBatch.Begin(transformMatrix: camera.GetTransformMatrix(screenCenter), samplerState: SamplerState.PointClamp);
            
            DrawMap(spriteBatch);
            DrawObjects(spriteBatch);
            DrawItemDrops(spriteBatch);
            DrawEnemies(spriteBatch);
            DrawPlayer(spriteBatch);
            
            spriteBatch.End();
        }

        private void DrawMap(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    Rectangle tileRect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                    TileType tile = map[x, y];
                    Color tileColor = tile switch
                    {
                        TileType.Grass => Color.ForestGreen,
                        TileType.Tree => Color.SaddleBrown,
                        TileType.Water => Color.DodgerBlue,
                        TileType.Stone => Color.Gray,
                        TileType.Chest => Color.DarkGoldenrod,
                        TileType.Exit => Color.Gold,
                        _ => Color.ForestGreen
                    };
                    spriteBatch.Draw(GetPixelTexture(spriteBatch), tileRect, tileColor);
                }
            }
        }

        private void DrawObjects(SpriteBatch spriteBatch)
        {
            foreach (var obj in objects)
            {
                Color objColor = obj.Type == "Vine" 
                    ? (obj.IsActivated ? Color.Brown : Color.DarkGreen)
                    : (obj.IsActivated ? Color.Yellow : Color.DarkGoldenrod);
                spriteBatch.Draw(GetPixelTexture(spriteBatch), obj.Bounds, objColor);
            }
        }

        private void DrawItemDrops(SpriteBatch spriteBatch)
        {
            foreach (var item in itemDrops)
            {
                Rectangle glowRect = new Rectangle(item.Bounds.X - 4, item.Bounds.Y - 4, 
                                                 item.Bounds.Width + 8, item.Bounds.Height + 8);
                spriteBatch.Draw(GetPixelTexture(spriteBatch), glowRect, item.DisplayColor * 0.3f);
                spriteBatch.Draw(GetPixelTexture(spriteBatch), item.Bounds, item.DisplayColor);
            }
        }

        private void DrawEnemies(SpriteBatch spriteBatch)
        {
            foreach (var enemy in enemies)
            {
                if (enemy.State == EntityState.Dead) continue;

                Rectangle enemyRect = new Rectangle((int)enemy.Position.X, (int)enemy.Position.Y, 32, 32);
                Color enemyColor = enemy.Type == EnemyType.Snake ? Color.LimeGreen : Color.DarkViolet;
                spriteBatch.Draw(GetPixelTexture(spriteBatch), enemyRect, enemyColor);

                // Health bar
                float healthPercent = (float)enemy.Health / enemy.MaxHealth;
                Rectangle healthBar = new Rectangle((int)enemy.Position.X, (int)enemy.Position.Y - 8, 
                                                  (int)(32 * healthPercent), 4);
                spriteBatch.Draw(GetPixelTexture(spriteBatch), healthBar, Color.Red);
            }
        }

        private void DrawPlayer(SpriteBatch spriteBatch)
        {
            if (Player.State == EntityState.Dead)
            {
                spriteBatch.Draw(GetPixelTexture(spriteBatch), Player.Bounds, Color.DarkRed);
            }
            else
            {
                spriteBatch.Draw(GetPixelTexture(spriteBatch), Player.Bounds, Color.Blue);
                
                if (Player.IsAttacking)
                {
                    Rectangle attackRect = new Rectangle((int)Player.Position.X - 10, (int)Player.Position.Y - 10, 52, 52);
                    DrawRect(spriteBatch, attackRect, Color.Red, 2);
                }
            }
        }

        private Texture2D GetPixelTexture(SpriteBatch spriteBatch)
        {
            if (pixelTexture == null)
            {
                pixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                pixelTexture.SetData(new[] { Color.White });
            }
            return pixelTexture;
        }

        private void DrawRect(SpriteBatch spriteBatch, Rectangle rect, Color color, int width)
        {
            spriteBatch.Draw(GetPixelTexture(spriteBatch), new Rectangle(rect.X, rect.Y, rect.Width, width), color);
            spriteBatch.Draw(GetPixelTexture(spriteBatch), new Rectangle(rect.X, rect.Bottom - width, rect.Width, width), color);
            spriteBatch.Draw(GetPixelTexture(spriteBatch), new Rectangle(rect.X, rect.Y, width, rect.Height), color);
            spriteBatch.Draw(GetPixelTexture(spriteBatch), new Rectangle(rect.Right - width, rect.Y, width, rect.Height), color);
        }

        private void CheckTileCollisions()
        {
            int playerTileX = (int)(Player.Position.X / tileSize);
            int playerTileY = (int)(Player.Position.Y / tileSize);

            for (int x = playerTileX - 1; x <= playerTileX + 1; x++)
            {
                for (int y = playerTileY - 1; y <= playerTileY + 1; y++)
                {
                    if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight) continue;

                    TileType tile = map[x, y];
                    Rectangle tileRect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);

                    bool isWaterBlocking = tile == TileType.Water;
                    if (tile == TileType.Water && x == 2 && y == 10 && vineIsPushed)
                        isWaterBlocking = false;

                    if ((tile == TileType.Tree || isWaterBlocking) && Player.Bounds.Intersects(tileRect))
                    {
                        Vector2 tileCenterVec = new Vector2(x * tileSize + tileSize / 2, y * tileSize + tileSize / 2);
                        Vector2 direction = Player.Position - tileCenterVec;
                        if (direction != Vector2.Zero)
                        {
                            direction.Normalize();
                            Player.Position += direction * 2f;
                        }
                    }
                }
            }
        }

        private void CheckPlayerAttack()
        {
            if (Player.HasDealtDamageThisAttack) return;

            foreach (var enemy in enemies)
            {
                if (enemy.State == EntityState.Dead) continue;

                float distance = Vector2.Distance(Player.Position, enemy.Position);
                if (distance < Player.AttackRange && Player.IsAttacking)
                {
                    int damage = Player.CalculateDamage(enemy.MaxHealth);
                    enemy.TakeDamage(damage);
                    Player.HasDealtDamageThisAttack = true;

                    if (enemy.State == EntityState.Dead && enemy.DropsKey)
                    {
                        bool shouldDropKey = false;

                        if (enemy.Type == EnemyType.Snake && snakesKilled == 0 && Player.GetKeyCount() == 0)
                        { shouldDropKey = true; snakesKilled++; }
                        else if (enemy.Type == EnemyType.Spider && spidersKilled == 0 && Player.GetKeyCount() == 1)
                        { shouldDropKey = true; spidersKilled++; }
                        else if (enemy.Type == EnemyType.Snake && snakesKilled == 1 && Player.GetKeyCount() == 2)
                        { shouldDropKey = true; snakesKilled++; }

                        if (shouldDropKey)
                        {
                            itemDrops.Add(new ItemDrop(enemy.Position, "Key"));
                            enemy.DropsKey = false;
                        }
                    }
                    break;
                }
            }
        }

        private void CheckItemPickups(KeyboardState keyState, KeyboardState prevKeyState)
        {
            for (int i = itemDrops.Count - 1; i >= 0; i--)
            {
                var item = itemDrops[i];
                float distance = Vector2.Distance(Player.Position, item.Position);

                if (distance < 40f && keyState.IsKeyDown(Keys.E) && prevKeyState.IsKeyUp(Keys.E))
                {
                    if (item.ItemName == "Key")
                    {
                        Player.AddToInventory(item.ItemName);
                    }
                    else if (item.ItemName == "Spiritvine Blade")
                    {
                        RelicItem spiritvineBlade = new RelicItem("Spiritvine Blade", Color.Cyan, 50);
                        Player.EquipRelic(spiritvineBlade);
                        Player.AddToInventory(item.ItemName);
                        hasRelicBlade = true;
                    }
                    itemDrops.RemoveAt(i);
                }
            }
        }

        private void CheckObjectInteractions(KeyboardState keyState, KeyboardState prevKeyState)
        {
            bool ePressed = keyState.IsKeyDown(Keys.E) && prevKeyState.IsKeyUp(Keys.E);

            foreach (var obj in objects)
            {
                float distance = Vector2.Distance(Player.Position, obj.Position);

                if (obj.Type == "Vine" && !obj.IsActivated && distance < 60f && ePressed)
                {
                    obj.Position.Y = tileSize * 10 - 16;
                    obj.IsActivated = true;
                    vineIsPushed = true;
                    map[2, 10] = TileType.Stone;
                }

                if (obj.Type == "Chest" && !obj.IsActivated && distance < 60f)
                {
                    if (Player.GetKeyCount() >= 3)
                    {
                        if (ePressed)
                        {
                            hasRelicBlade = true;
                            obj.IsActivated = true;
                            itemDrops.Add(new ItemDrop(new Vector2(obj.Position.X + 20, obj.Position.Y + 40), "Spiritvine Blade"));
                        }
                    }
                }
            }
        }

        private void RespawnEnemies()
        {
            foreach (var enemy in enemies.Where(e => e.State == EntityState.Dead))
            {
                int x, y;
                do
                {
                    x = random.Next(2, mapWidth - 2);
                    y = random.Next(2, 8);
                } while (map[x, y] == TileType.Water || map[x, y] == TileType.Tree);

                enemy.Position = new Vector2(x * tileSize, y * tileSize);
                enemy.Health = enemy.MaxHealth;
                enemy.State = EntityState.Idle;
            }
        }

        private void CheckExitReached()
        {
            if (!Player.HasItem("Spiritvine Blade")) return;

            Rectangle exitRect = new Rectangle(14 * tileSize, 13 * tileSize, tileSize, tileSize);
            if (Player.Bounds.Intersects(exitRect))
            {
                IsCompleted = true;
                System.Diagnostics.Debug.WriteLine("LEVEL 1 COMPLETED!");
            }
        }
    }
}