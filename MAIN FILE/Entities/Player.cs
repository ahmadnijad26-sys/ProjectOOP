// Entities/Player.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace ProjectOOPGame_Fresh
{
    public class Player
    {
        public Vector2 Position;
        public int Health, MaxHealth;
        public List<string> Inventory;
        public List<RelicItem> EquippedRelics;
        public EntityState State;
        public float Speed;
        public Rectangle Bounds;
        public bool IsAttacking;
        public float AttackCooldown;
        public int AttackRange;
        public RelicItem CurrentWeapon;
        public bool HasDealtDamageThisAttack;
        public SpriteEffects FacingDirection;

        public Player(Vector2 startPos)
        {
            Position = startPos;
            Health = MaxHealth = 100;
            Inventory = new List<string>();
            EquippedRelics = new List<RelicItem>();
            State = EntityState.Idle;
            Speed = 150f;
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
            IsAttacking = false;
            AttackCooldown = 0f;
            AttackRange = 50;
            HasDealtDamageThisAttack = false;
            FacingDirection = SpriteEffects.None;
            CurrentWeapon = new RelicItem("Combat Knife", Color.Gray, 30);
        }

        public void Update(float deltaTime, KeyboardState keyState, KeyboardState prevKeyState)
        {
            if (State == EntityState.Dead) return;

            if (AttackCooldown > 0) AttackCooldown -= deltaTime;

            Vector2 movement = Vector2.Zero;
            if (keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Up)) movement.Y -= 1;
            if (keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down)) movement.Y += 1;
            if (keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left))
            {
                movement.X -= 1;
                FacingDirection = SpriteEffects.FlipHorizontally;
            }
            if (keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right))
            {
                movement.X += 1;
                FacingDirection = SpriteEffects.None;
            }

            if (movement != Vector2.Zero)
            {
                movement.Normalize();
                Position += movement * Speed * deltaTime;
                State = EntityState.Walking;
            }
            else State = EntityState.Idle;

            if (keyState.IsKeyDown(Keys.Space) && prevKeyState.IsKeyUp(Keys.Space) && AttackCooldown <= 0)
            {
                IsAttacking = true;
                HasDealtDamageThisAttack = false;
                AttackCooldown = 0.5f;
            }
            else if (AttackCooldown <= 0)
            {
                IsAttacking = false;
                HasDealtDamageThisAttack = false;
            }

            Bounds = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0) { Health = 0; State = EntityState.Dead; }
        }

        public void AddToInventory(string item) => Inventory.Add(item);

        public void EquipRelic(RelicItem relic)
        {
            if (relic.Name.Contains("Blade") || relic.Name.Contains("Knife"))
                CurrentWeapon = relic;
            else
                EquippedRelics.Add(relic);
        }

        public int GetKeyCount() => Inventory.Count(item => item == "Key");
        public bool HasItem(string itemName) => Inventory.Contains(itemName);
        public int CalculateDamage(int enemyMaxHP) => (int)(enemyMaxHP * (CurrentWeapon.DamagePercent / 100f));
    }
}