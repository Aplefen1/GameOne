using System.Collections.Generic;
using System.Text;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FirstGame.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FirstGame.Entities
{

    public class GameOverScreen : IGameEntity
    {
        private const int GAME_OVER_TEXTURE_POS_X = 655;
        private const int GAME_OVER_TEXTURE_POS_Y = 14;
        public const int GAME_OVER_TEXTURE_WIDTH = 192;
        public const int GAME_OVER_TEXTURE_HEIGHT = 14;

        private const int BUTTON_TEXTURE_POS_X = 1;
        private const int BUTTON_TEXTURE_POS_Y = 1;
        private const int BUTTON_TEXTURE_WIDTH = 38;
        private const int BUTTON_TEXTURE_HEIGHT = 34;

        private Sprite _textSprite;
        private Sprite _buttonSprite;

        private KeyboardState _previousKeyboardState;

        public bool IsEnabled { get; set; }

        public Vector2 Position { get; set; }
        private Vector2 ButtonPostion => Position + new Vector2((GAME_OVER_TEXTURE_WIDTH/2 - BUTTON_TEXTURE_WIDTH/2),  GAME_OVER_TEXTURE_HEIGHT * 2);
        private Rectangle ButtonBounds
            => new Rectangle(ButtonPostion.ToPoint(), new Point(BUTTON_TEXTURE_WIDTH, BUTTON_TEXTURE_HEIGHT));

        private TrexRunner _trexRunnerGame;
 
        public int DrawOrder => 100;

        public GameOverScreen(Texture2D spriteSheet, Vector2 position, TrexRunner trexRunnerGame)
        {
            _textSprite = new Sprite(
                spriteSheet, 
                GAME_OVER_TEXTURE_POS_X, 
                GAME_OVER_TEXTURE_POS_Y, 
                GAME_OVER_TEXTURE_WIDTH, 
                GAME_OVER_TEXTURE_HEIGHT
                );

            _buttonSprite = new Sprite(
                spriteSheet, 
                BUTTON_TEXTURE_POS_X, 
                BUTTON_TEXTURE_POS_Y, 
                BUTTON_TEXTURE_WIDTH, 
                BUTTON_TEXTURE_HEIGHT
                );

            Position = position;
            _trexRunnerGame = trexRunnerGame;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsEnabled)
                return;

            MouseState mouseState = Mouse.GetState();
            KeyboardState keyboardState = Keyboard.GetState();

            bool isKeyPressed = keyboardState.IsKeyDown(Keys.Space) || keyboardState.IsKeyDown(Keys.Up);
            bool wasKeyPressed = _previousKeyboardState.IsKeyDown(Keys.Space) || _previousKeyboardState.IsKeyDown(Keys.Up);

            if((ButtonBounds.Contains(mouseState.Position) && mouseState.LeftButton == ButtonState.Pressed)
                || (wasKeyPressed && !isKeyPressed))
            {
                _trexRunnerGame.Replay();

            }
            _previousKeyboardState = keyboardState;

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (!IsEnabled)
                return;
            
            _textSprite.Draw(spriteBatch, Position);
            _buttonSprite.Draw(spriteBatch, ButtonPostion);

        }
    }
    
}