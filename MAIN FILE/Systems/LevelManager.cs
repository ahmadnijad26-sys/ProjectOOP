using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace ProjectOOPGame_Fresh
{
    public class LevelManager
    {
        public Level CurrentLevel { get; private set; }
        public Action<string> OnShowMessage { get; set; }
        
        private Dictionary<string, Level> levels;
        private string currentLevelName;

        public LevelManager()
        {
            levels = new Dictionary<string, Level>();
        }

        public void Initialize(Player player)
        {
            // Create your levels
            levels.Clear();
            
            // Add your levels here - you'll need to create these level classes
            levels["Tutorial"] = new TutoLevel(player);
            levels["Level1"] = new Level1(player);
            // Add more levels as needed...
            
            // Start with first level
            currentLevelName = "Tutorial";
            CurrentLevel = levels[currentLevelName];
            
            // Connect message system
            foreach (var level in levels.Values)
            {
                level.OnShowMessage = OnShowMessage;
            }
        }

        public void Update(float deltaTime, KeyboardState currentKeyState, KeyboardState previousKeyState)
        {
            if (CurrentLevel != null)
            {
                CurrentLevel.Update(deltaTime, currentKeyState, previousKeyState);
                
                // Check for level completion and transition to next level
                if (CurrentLevel.IsCompleted)
                {
                    // Transition to next level logic here
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            if (CurrentLevel != null)
            {
                CurrentLevel.Draw(spriteBatch, camera);
            }
        }

        public void ChangeLevel(string levelName)
        {
            if (levels.ContainsKey(levelName))
            {
                currentLevelName = levelName;
                CurrentLevel = levels[levelName];
            }
        }
    }
}
