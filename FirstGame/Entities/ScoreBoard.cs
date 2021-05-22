using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace FirstGame.Entities
{
    public class ScoreBoard : IGameEntity
    {
        private const int TEXTURE_COORDS_X = 655;
        private const int TEXTURE_COORDS_Y = 0;
        private const int TEXTURE_COORDS_WIDTH = 10;
        private const int TEXTURE_COORDS_HEIGHT = 13;

        private const byte NUMBER_OF_DIGITS_TO_DRAW = 5;

        private const int TEXTURE_COORDS_HI_X = 755;
        private const int TEXTURE_COORDS_HI_Y = 0;
        private const int TEXTURE_COORDS_HI_WIDTH = 20;
        private const int HI_TEXT_MARGIN = 25;
        private const int SCORE_MARGIN = 70;
        private const int SCORE_INCREMENT_MULTIPLIER = 10;

        private const float FLASH_ANIM_FRAME_LENGTH = 0.2f;
        private const int FLASH_ANIM_FLASH_COUNT = 8;

        private Texture2D _texture;
        private bool _isPlayingFlash;
        private float _flashAnimationTime;
        private SoundEffect _sfxMilestone;

        public double Score { get; set; }

        public int DisplayScore => (int)Math.Floor(Score);

        public int HighScore { get; set; }

        public bool HasHighScore => HighScore > 0;

        private Trex _trex;

        public int DrawOrder => 100;

        public Vector2 Position { get; set; }

        public ScoreBoard(Texture2D texture, Vector2 position, Trex trex, SoundEffect sfx)
        {
            _texture = texture;
            Position = position;
            _trex = trex;
            _sfxMilestone = sfx;

            _trex.Died += SetHighScore;
        }

        public void Update(GameTime gameTime)
        {

            IncreaseScore(gameTime);
            FlashAnimation(gameTime);

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            if(HasHighScore)
            {

                spriteBatch.Draw(_texture, new Vector2(Position.X - HI_TEXT_MARGIN, Position.Y), new Rectangle(TEXTURE_COORDS_HI_X,TEXTURE_COORDS_HI_Y,TEXTURE_COORDS_HI_WIDTH,TEXTURE_COORDS_HEIGHT), Color.White);
                DrawScore(spriteBatch, HighScore, Position.X);

            }

            if(!_isPlayingFlash || (int)(_flashAnimationTime / FLASH_ANIM_FRAME_LENGTH) % 2 == 0)
                DrawScore(spriteBatch, DisplayScore, Position.X + SCORE_MARGIN);

        }

        public void DrawScore(SpriteBatch spriteBatch, int score, float startPosX)
        {

            int[] scoreDigits = SplitDigits(score);

            float posX = startPosX;

            foreach(int digit in scoreDigits)
            {
                Rectangle textureCoords = GetDigitTextureBounds(digit);

                Vector2 screenPos = new Vector2(posX, Position.Y);

                spriteBatch.Draw(_texture, screenPos, textureCoords, Color.White);

                posX += TEXTURE_COORDS_WIDTH;
            }

        }

        private int[] SplitDigits(int input)
        {

            string inputStr = input.ToString().PadLeft(NUMBER_OF_DIGITS_TO_DRAW, '0');

            int[] result = new int[inputStr.Length];

            for(int i = 0; i < result.Length; i++)
            {
                result[i] = (int)char.GetNumericValue(inputStr[i]);
            }

            return result;
        }

        private Rectangle GetDigitTextureBounds(int digit)
        {
            if(digit < 0 || digit > 9)
                throw new ArgumentOutOfRangeException("digit", "Out of range digit (0-9)");
            
            int posX = TEXTURE_COORDS_X + digit * TEXTURE_COORDS_WIDTH;
            int posY = TEXTURE_COORDS_Y;

            return new Rectangle(posX, posY, TEXTURE_COORDS_WIDTH, TEXTURE_COORDS_HEIGHT); 

        }

        private void IncreaseScore(GameTime gameTime)
        {

            Score += gameTime.ElapsedGameTime.TotalSeconds * (_trex.Speed / _trex.InitialSpeed) * SCORE_INCREMENT_MULTIPLIER;
            
        }

        public void Initialize()
        {

            Score = 0;
            HighScore = 0;

        }

        public void FlashAnimation(GameTime gameTime)
        {
            int oldScore = DisplayScore;
            if(!_isPlayingFlash && DisplayScore % 100 == 0 && DisplayScore >= 100 )
            {
                _sfxMilestone.Play();

                _isPlayingFlash = true;
                _flashAnimationTime = 0;

            }

            if(_isPlayingFlash)
            {
                _flashAnimationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if(_flashAnimationTime >= FLASH_ANIM_FLASH_COUNT * FLASH_ANIM_FRAME_LENGTH)
                {
                    _isPlayingFlash = false;
                }
            }
        }
        private void SetHighScore(object sender, EventArgs e)
        {

            if(DisplayScore > HighScore)
            {
                
                HighScore = DisplayScore;

            }

        }
    }

}