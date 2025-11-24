// Systems/LevelManager.cs
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectOOPGame_Fresh
{
    public class LevelManager
    {
        private List<IGameLevel> levels;
        private IGameLevel currentLevel;
        private int currentLevelIndex = 0;

        public LevelManager()
        {
            levels = new List<IGameLevel>();
        }

        public void Initialize(Player player)
        {
            // Create all levels
            levels.Add(new Level1(player));
            //levels.Add(new Level2(player));
            //levels.Add(new Level3(player));
            
            // Start with level 1
            currentLevel = levels[0];
            currentLevel.Initialize();
        }

        public void Update(float deltaTime, KeyboardState keyState, KeyboardState prevKeyState)
        {
            currentLevel.Update(deltaTime, keyState, prevKeyState);
            
            // Check if level completed
            if (currentLevel.IsCompleted)
            {
                currentLevelIndex++;
                if (currentLevelIndex < levels.Count)
                {
                    currentLevel = levels[currentLevelIndex];
                    currentLevel.Initialize();
                }
                else
                {
                    // Game completed - all levels finished
                    System.Diagnostics.Debug.WriteLine("ALL LEVELS COMPLETED!");
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            currentLevel.Draw(spriteBatch, camera);
        }

        public IGameLevel CurrentLevel => currentLevel;
        public int CurrentLevelNumber => currentLevelIndex + 1;
    }
}