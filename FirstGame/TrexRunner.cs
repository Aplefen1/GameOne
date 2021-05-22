using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using FirstGame.Graphics;
using FirstGame.Entities;
using FirstGame.System;
using System;

namespace FirstGame
{
    public class TrexRunner : Game
    {
        private const string ASSET_NAME_SPRITESHEET = "TrexSpritesheet";
        private const string ASSET_NAME_SFX_HIT = "hit";
        private const string ASSET_NAME_SFX_SCORE_REACHED = "score-reached";
        private const string ASSET_NAME_SFX_BUTTON_PRESS = "button-press";

        public const int WINDOW_WIDTH = 600;
        public const int WINDOW_HEIGHT = 150;

        public const int TREX_START_POS_Y = WINDOW_HEIGHT-16;
        public const int TREX_START_POS_X = 1;
        public const int GROUND_START_POS_Y = WINDOW_HEIGHT-31;

        private const int SCORE_BOARD_POS_X = WINDOW_WIDTH - 130;
        private const int SCORE_BOARD_POS_Y = 10;
        
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        private SoundEffect sfxButtonPress;
        private SoundEffect sfxScoreReached;
        private SoundEffect sfxHit;

        private Texture2D spriteSheetTexture;
        private Texture2D _fadeInTexture;
        private float _fadeInTexturePosX;

        private Trex trex;
        private ScoreBoard _scoreBoard;

        private InputController _inputController;
        private GroundManager _groundManager;
        private EntityManager _entityManager;
        private ObstacleManager _obstacleManager;
        private GameOverScreen _gameOverScreen;
        private BackGroundManager _backroundManager;

        public GameState State { get; private set; }

        public string FPS
        {
            get
            {
                return Math.Round(1f/_frameDiff).ToString();
            }
        }

        private double _frameDiff;


        public TrexRunner()
        {

            IsFixedTimeStep = false;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _entityManager = new EntityManager();
            State = GameState.Initial;
            _fadeInTexturePosX = Trex.TREX_DEF_SPRITE_WIDTH;

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            _graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            sfxButtonPress = Content.Load<SoundEffect>(ASSET_NAME_SFX_BUTTON_PRESS);
            sfxScoreReached = Content.Load<SoundEffect>(ASSET_NAME_SFX_SCORE_REACHED);
            sfxHit = Content.Load<SoundEffect>(ASSET_NAME_SFX_HIT);

            _font = Content.Load<SpriteFont>("MyFont");

            spriteSheetTexture = Content.Load<Texture2D>(ASSET_NAME_SPRITESHEET);

            _fadeInTexture = new Texture2D(GraphicsDevice, 1, 1);
            _fadeInTexture.SetData(new Color[] { Color.White });

            trex = new Trex(spriteSheetTexture, new Vector2(TREX_START_POS_X, TREX_START_POS_Y - Trex.TREX_DEF_SPRITE_HEIGHT), sfxButtonPress, GraphicsDevice);
            trex.DrawOrder = 10;

            trex.JumpComplete += TrexJumpComplete;
            trex.Died += TrexDied;

            _scoreBoard = new ScoreBoard(spriteSheetTexture, new Vector2(SCORE_BOARD_POS_X, SCORE_BOARD_POS_Y), trex, sfxScoreReached);

            _inputController = new InputController(trex);

            _groundManager = new GroundManager(spriteSheetTexture, _entityManager, trex);

            _obstacleManager = new ObstacleManager(_entityManager, trex, _scoreBoard, spriteSheetTexture, GraphicsDevice);

            _backroundManager = new BackGroundManager(spriteSheetTexture, _entityManager, trex);

            _gameOverScreen = new GameOverScreen(
                spriteSheetTexture, 
                new Vector2(
                (WINDOW_WIDTH - GameOverScreen.GAME_OVER_TEXTURE_WIDTH)/2, 
                (WINDOW_HEIGHT - GameOverScreen.GAME_OVER_TEXTURE_HEIGHT)/3
                ),
                this
                );

            _entityManager.AddEntity(trex);
            _entityManager.AddEntity(_groundManager);
            _entityManager.AddEntity(_scoreBoard);
            _entityManager.AddEntity(_obstacleManager);
            _entityManager.AddEntity(_gameOverScreen);
            _entityManager.AddEntity(_backroundManager);

        }

        protected override void Update(GameTime gameTime)
        {
            _frameDiff = gameTime.ElapsedGameTime.TotalSeconds;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);

            KeyboardState keyboardState = Keyboard.GetState();
            

            if(State == GameState.Initial)
            {

                bool isStartKeyPressed = keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.Space);
                if(isStartKeyPressed)
                    StartGame();

            }

            if(State == GameState.Playing)
            {
                _inputController.ProcessControls(gameTime);
            }

            else if (State == GameState.Transition)
            {

                _fadeInTexturePosX += (float)gameTime.ElapsedGameTime.TotalSeconds * 700f;
                if(_fadeInTexturePosX > WINDOW_WIDTH)
                    State = GameState.Playing;

            }
                

            _entityManager.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            //All draw calls will be queued until .end() is called
            _entityManager.Draw(_spriteBatch, gameTime);

            if(State == GameState.Initial || State == GameState.Transition)
            {

                _spriteBatch.Draw(_fadeInTexture, new Rectangle((int)Math.Round(_fadeInTexturePosX), 0 , WINDOW_WIDTH, WINDOW_HEIGHT), Color.White);

            }

            _spriteBatch.DrawString(_font, FPS, new Vector2(10,10), Color.Black);

            _spriteBatch.End();
            
            base.Draw(gameTime);
        }

        private bool StartGame()
        {

            if(State != GameState.Initial)
                return false;
            
            State = GameState.Transition;
            trex.BeginJump();
            _groundManager.Initialize();
            _backroundManager.Initilaize();

            return true;

        }

        private void TrexJumpComplete(object sender, EventArgs e)
        {

            if(State == GameState.Transition)
            {

                State = GameState.Playing;
                trex.Initialize();
                _scoreBoard.Initialize();

            }

        }

        private void TrexDied(object sender, EventArgs e)
        {

            State = GameState.GameOver;
            _obstacleManager.IsEnabled = false;
            _gameOverScreen.IsEnabled = true;
            _backroundManager.Stop();
            sfxHit.Play();

        }

        public bool Replay()
        {
            if (State != GameState.GameOver)
                return false;
            
            State = GameState.Playing;

            _obstacleManager.IsEnabled = true;
            _obstacleManager.Reset();

            _gameOverScreen.IsEnabled = false;

            _groundManager.Initialize();
            _backroundManager.Initilaize();

            _scoreBoard.Score = 0;

            _inputController.BlockInputTemporarily();

            trex.Initialize();

            return true;
        }
    }
}
