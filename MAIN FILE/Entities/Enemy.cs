// Entities/Enemy.cs
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;  // For SpriteEffects

namespace ProjectOOPGame_Fresh
{
    public class Enemy
    {
        public Vector2 Position;
        public EnemyType Type;
        public int Health, MaxHealth;
        public EntityState State;
        public float Speed;
        public Rectangle Bounds;
        public float DetectionRange, AttackRange, AttackCooldown;
        public bool DropsKey;
        public Vector2 WanderTarget;
        public float WanderTimer;
        public SpriteEffects FacingDirection;

        public Enemy(Vector2 startPos, EnemyType type, bool dropsKey = false)
        {
            Position = startPos;
            Type = type;
            MaxHealth = type == EnemyType.Snake ? 30 : 25;
            Health = MaxHealth;
            State = EntityState.Idle;
            Speed = type == EnemyType.Snake ? 60f : 50f;
            Bounds = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
            DetectionRange = 200f;
            AttackRange = 35f;
            AttackCooldown = 0f;
            DropsKey = dropsKey;
            WanderTarget = startPos;
            WanderTimer = 0f;
            FacingDirection = SpriteEffects.None;
        }

        public void Update(float deltaTime, Player player, Random random)
        {
            if (State == EntityState.Dead) return;

            float distanceToPlayer = Vector2.Distance(Position, player.Position);
            if (AttackCooldown > 0) AttackCooldown -= deltaTime;

            if (distanceToPlayer < DetectionRange)
            {
                Vector2 direction = player.Position - Position;
                if (direction != Vector2.Zero)
                {
                    FacingDirection = direction.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    direction.Normalize();
                    Position += direction * Speed * deltaTime;
                    State = EntityState.Walking;
                }

                if (distanceToPlayer < AttackRange && AttackCooldown <= 0 && player.State != EntityState.Dead)
                {
                    player.TakeDamage(5);
                    AttackCooldown = 1.5f;
                }
            }
            else
            {
                WanderTimer -= deltaTime;
                if (WanderTimer <= 0)
                {
                    WanderTarget = Position + new Vector2(random.Next(-100, 100), random.Next(-100, 100));
                    WanderTimer = random.Next(2, 5);
                }

                Vector2 direction = WanderTarget - Position;
                if (direction.Length() > 5f)
                {
                    FacingDirection = direction.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    direction.Normalize();
                    Position += direction * Speed * 0.5f * deltaTime;
                    State = EntityState.Walking;
                }
                else State = EntityState.Idle;
            }

            Bounds = new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0) { Health = 0; State = EntityState.Dead; }
        }
    }
}