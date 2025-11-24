using Microsoft.Xna.Framework;
using System;

namespace ProjectOOPGame_Fresh
{
    public class Camera
    {
        private Vector2 position;
        private Vector2 shakeOffset;
        private float shakeDuration = 0f;
        private float shakeIntensity = 0f;
        private Random random = new Random();

        public void Update(float deltaTime, Vector2 targetPosition, int mapWidth, int mapHeight, int tileSize, Vector2 screenCenter)
        {
            // Update shake effect
            if (shakeDuration > 0)
            {
                shakeDuration -= deltaTime;
                float shakeAmount = shakeIntensity * (shakeDuration / 0.2f);
                shakeOffset = new Vector2(
                    (float)(random.NextDouble() * 2 - 1) * shakeAmount,
                    (float)(random.NextDouble() * 2 - 1) * shakeAmount
                );
                if (shakeDuration <= 0) shakeOffset = Vector2.Zero;
            }

            // Follow target with bounds checking
            position = targetPosition + shakeOffset;
            
            float minX = screenCenter.X / 2.0f;
            float maxX = (mapWidth * tileSize) - screenCenter.X / 2.0f;
            float minY = screenCenter.Y / 2.0f;
            float maxY = (mapHeight * tileSize) - screenCenter.Y / 2.0f;

            position.X = MathHelper.Clamp(position.X, minX, maxX);
            position.Y = MathHelper.Clamp(position.Y, minY, maxY);
        }

        public void Shake(float duration, float intensity)
        {
            shakeDuration = duration;
            shakeIntensity = intensity;
        }

        public Matrix GetViewMatrix(Vector2 screenCenter, float zoom = 2.0f)
        {
            return Matrix.CreateTranslation(-position.X, -position.Y, 0) *
                   Matrix.CreateScale(zoom) *
                   Matrix.CreateTranslation(screenCenter.X, screenCenter.Y, 0);
        }
    }
}
