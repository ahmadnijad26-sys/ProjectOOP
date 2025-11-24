using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ProjectOOPGame_Fresh
{
    public abstract class Level
    {
        public Player Player { get; protected set; }
        public List<Enemy> Enemies { get; protected set; }
        public List<InteractiveObject> Objects { get; protected set; }
        public List<ItemDrop> ItemDrops { get; protected set; }
        public TileType[,] Map { get; protected set; }
        public bool IsCompleted { get; protected set; }
        public Action<string> OnShowMessage { get; set; }

        protected int mapWidth = 16;
        protected int mapHeight = 16;
        protected int tileSize = 64;
        protected Random random = new Random();

        public Level(Player player)
        {
            Player = player;
            Enemies = new List<Enemy>();
            Objects = new List<InteractiveObject>();
            ItemDrops = new List<ItemDrop>();
            InitializeMap();
            InitializeEnemies();
            InitializeObjects();
        }

        protected abstract void InitializeMap();
        protected abstract void InitializeEnemies();
        protected abstract void InitializeObjects();

        public virtual void Update(float deltaTime, KeyboardState currentKeyState, KeyboardState previousKeyState)
        {
            // Update player
            Player.Update(deltaTime, currentKeyState, previousKeyState);

            // Update enemies
            foreach (var enemy in Enemies)
            {
                enemy.Update(deltaTime, Player, random);
            }

            // Check collisions, attacks, etc.
            CheckPlayerAttack();
            CheckItemPickups(currentKeyState, previousKeyState);
            CheckObjectInteractions(currentKeyState, previousKeyState);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            spriteBatch.Begin(transformMatrix: camera.GetViewMatrix(new Vector2(640, 360)), 
                            samplerState: SamplerState.PointClamp);
            
            // Draw map
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    Rectangle tileRect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                    // Draw tile based on Map[x,y] - you'll need to implement this
                }
            }

            // Draw objects, items, enemies, player
            foreach (var obj in Objects)
            {
                // Draw object
            }

            foreach (var item in ItemDrops)
            {
                // Draw item
            }

            foreach (var enemy in Enemies)
            {
                if (enemy.State != EntityState.Dead)
                {
                    // Draw enemy
                }
            }

            // Draw player
            // ...

            spriteBatch.End();
        }

        protected virtual void CheckPlayerAttack()
        {
            if (Player.IsAttacking && !Player.HasDealtDamageThisAttack)
            {
                foreach (var enemy in Enemies)
                {
                    if (enemy.State != EntityState.Dead && 
                        Vector2.Distance(Player.Position, enemy.Position) < Player.AttackRange)
                    {
                        int damage = Player.CalculateDamage(enemy.MaxHealth);
                        enemy.TakeDamage(damage);
                        Player.HasDealtDamageThisAttack = true;

                        if (enemy.State == EntityState.Dead && enemy.DropsKey)
                        {
                            ItemDrops.Add(new ItemDrop(enemy.Position, "Key"));
                            OnShowMessage?.Invoke("Key dropped! Press E to pick up");
                        }
                        break;
                    }
                }
            }
        }

        protected virtual void CheckItemPickups(KeyboardState currentKeyState, KeyboardState previousKeyState)
        {
            for (int i = ItemDrops.Count - 1; i >= 0; i--)
            {
                var item = ItemDrops[i];
                if (Vector2.Distance(Player.Position, item.Position) < 40f && 
                    currentKeyState.IsKeyDown(Keys.E) && previousKeyState.IsKeyUp(Keys.E))
                {
                    if (item.ItemName == "Key")
                    {
                        Player.AddToInventory("Key");
                        OnShowMessage?.Invoke($"Picked up Key! ({Player.GetKeyCount()}/3 keys)");
                    }
                    ItemDrops.RemoveAt(i);
                }
            }
        }

        protected virtual void CheckObjectInteractions(KeyboardState currentKeyState, KeyboardState previousKeyState)
        {
            // Your object interaction logic here
        }
    }
}
