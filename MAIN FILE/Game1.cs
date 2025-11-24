using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ProjectOOPGame_Fresh
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private GameState currentGameState;
        private LevelManager levelManager;
        private Camera camera;
        private InputManager inputManager;

        private float damageFlashAlpha = 0f;
        private Texture2D pixelTexture;
        private SpriteFont font;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize()
        {
            // Create player that persists across levels
            Player player = new Player(new Vector2(128, 128));
            
            // Initialize level management
            levelManager = new LevelManager();
            levelManager.Initialize(player);
            
            camera = new Camera();
            inputManager = new InputManager();
            
            currentGameState = GameState.Playing;
            
            base.Initialize();

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });
            
            try
            {
                font = Content.Load<SpriteFont>("Font");
            }
            catch
            {
                font = null;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            inputManager.Update();

            switch (currentGameState)
            {
                case GameState.Playing:
                    UpdatePlaying(deltaTime);
                    break;
                case GameState.GameOver:
                    UpdateGameOver();
                    break;
            }

            if (damageFlashAlpha > 0)
            {
                damageFlashAlpha -= deltaTime * 5f;
                if (damageFlashAlpha < 0) damageFlashAlpha = 0;
            }

            base.Update(gameTime);
        }

        private void UpdatePlaying(float deltaTime)
        {
            levelManager.Update(deltaTime, inputManager.CurrentState, inputManager.PreviousState);
            
            // Check if player died
            if (levelManager.CurrentLevel.Player.State == EntityState.Dead)
            {
                currentGameState = GameState.GameOver;
                return;
            }

            // Update camera to follow player
            var screenCenter = new Vector2(1280 / 2f, 720 / 2f);
            camera.Update(deltaTime, levelManager.CurrentLevel.Player.Position, 
                        16, 16, 64, screenCenter);
        }

        private void UpdateGameOver()
        {
            if (inputManager.IsKeyPressed(Keys.R))
            {
                RestartGame();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (currentGameState)
            {
                case GameState.Playing:
                    DrawPlaying();
                    break;
                case GameState.GameOver:
                    DrawGameOver();
                    break;
            }

            base.Draw(gameTime);
        }

        private void DrawPlaying()
        {
            // Let the current level handle its own drawing
            levelManager.Draw(spriteBatch, camera);

            // Draw damage flash effect
            if (damageFlashAlpha > 0)
            {
                spriteBatch.Begin();
                DrawDamageFlash();
                spriteBatch.End();
            }

            // Draw UI
            spriteBatch.Begin();
            DrawUI();
            spriteBatch.End();
        }

        private void DrawGameOver()
        {
            spriteBatch.Begin();
            Rectangle fullScreen = new Rectangle(0, 0, 1280, 720);
            spriteBatch.Draw(pixelTexture, fullScreen, Color.Black);

            if (font != null)
            {
                string gameOverText = "GAME OVER";
                Vector2 gameOverSize = font.MeasureString(gameOverText);
                Vector2 gameOverPos = new Vector2(640 - gameOverSize.X / 2, 360 - 50);
                spriteBatch.DrawString(font, gameOverText, gameOverPos, Color.Red);

                string instructionText = "Press R to Restart";
                Vector2 instructionSize = font.MeasureString(instructionText);
                Vector2 instructionPos = new Vector2(640 - instructionSize.X / 2, 360 + 20);
                spriteBatch.DrawString(font, instructionText, instructionPos, Color.White);
            }
            spriteBatch.End();
        }

        private void DrawDamageFlash()
        {
            int edge = 40;
            spriteBatch.Draw(pixelTexture, new Rectangle(0, 0, 1280, edge), Color.Red * damageFlashAlpha);
            spriteBatch.Draw(pixelTexture, new Rectangle(0, 720 - edge, 1280, edge), Color.Red * damageFlashAlpha);
            spriteBatch.Draw(pixelTexture, new Rectangle(0, 0, edge, 720), Color.Red * damageFlashAlpha);
            spriteBatch.Draw(pixelTexture, new Rectangle(1280 - edge, 0, edge, 720), Color.Red * damageFlashAlpha);
        }

        private void DrawUI()
        {

            Player player = levelManager.CurrentLevel.Player;
            
            Rectangle uiPanel = new Rectangle(10, 10, 350, 180);
            spriteBatch.Draw(pixelTexture, uiPanel, Color.Black * 0.7f);

            // Health bar
            Rectangle healthBarBg = new Rectangle(20, 20, 200, 20);
            Rectangle healthBarFg = new Rectangle(20, 20, (int)(200 * (player.Health / 100f)), 20);
            spriteBatch.Draw(pixelTexture, healthBarBg, Color.DarkRed);
            spriteBatch.Draw(pixelTexture, healthBarFg, Color.Green);

            if (font != null)
            {
                spriteBatch.DrawString(font, $"HP: {player.Health}/100", new Vector2(25, 22), Color.White);
                spriteBatch.DrawString(font, "WASD: Move | Space: Attack | E: Interact",
                    new Vector2(10, 690), Color.White);
            }

            // Keys display
            for (int i = 0; i < player.GetKeyCount(); i++)
            {
                Rectangle keyBox = new Rectangle(25 + i * 35, 55, 30, 30);
                spriteBatch.Draw(pixelTexture, keyBox, Color.Gold);
            }
        }

        private void RestartGame()
        {
            currentGameState = GameState.Playing;
            Player player = new Player(new Vector2(128, 128));
            levelManager.Initialize(player);
            damageFlashAlpha = 0f;
        }
    }
}