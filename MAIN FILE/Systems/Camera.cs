// Systems/Camera.cs
using System;  // For Random
using Microsoft.Xna.Framework;


namespace ProjectOOPGame_Fresh
{
    public class Camera
    {
        public Vector2 Position;
        public float Zoom { get; set; } = 2.0f;
        public Vector2 ShakeOffset;
        public float ShakeIntensity = 0f;
        public float ShakeDuration = 0f;

        private Random random = new Random();

        public void Update(float deltaTime, Vector2 targetPosition, int mapWidth, int mapHeight, int tileSize, Vector2 screenCenter)
        {
            // Update shake
            if (ShakeDuration > 0)
            {
                ShakeDuration -= deltaTime;
                float shakeAmount = ShakeIntensity * (ShakeDuration / 0.2f);
                ShakeOffset = new Vector2(
                    (float)(random.NextDouble() * 2 - 1) * shakeAmount,
                    (float)(random.NextDouble() * 2 - 1) * shakeAmount
                );
                if (ShakeDuration <= 0) ShakeOffset = Vector2.Zero;
            }

            // Update position
            Position = targetPosition + ShakeOffset;

            // Clamp to map boundaries
            float minX = screenCenter.X / Zoom;
            float maxX = (mapWidth * tileSize) - screenCenter.X / Zoom;
            float minY = screenCenter.Y / Zoom;
            float maxY = (mapHeight * tileSize) - screenCenter.Y / Zoom;

            Position.X = MathHelper.Clamp(Position.X, minX, maxX);
            Position.Y = MathHelper.Clamp(Position.Y, minY, maxY);
        }

        public Matrix GetTransformMatrix(Vector2 screenCenter)
        {
            return Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
                   Matrix.CreateScale(Zoom) *
                   Matrix.CreateTranslation(screenCenter.X, screenCenter.Y, 0);
        }

        public void TriggerShake(float intensity, float duration)
        {
            ShakeIntensity = intensity;
            ShakeDuration = duration;
        }
    }
}