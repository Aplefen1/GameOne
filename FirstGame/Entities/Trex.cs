using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FirstGame.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace FirstGame.Entities
{ 
    public class Trex : IGameEntity, ICollidable
    {

        //Physics/movement constants
        private const float GRAVITY = 1600f;
        private const float JUMP_START_VELOCITY = -480f;
        private const float CANCEL_JUMP_VELOCITY = -120;
        private const float MIN_JUMP_HEIGHT = 40;
        private const float DROP_VELOCITY = 550f;
        private const float START_SPEED = 500f;
        private const float MAX_SPEED = 1200f;
        private const float SPEED_ACCELERATION = 8;
        private const int COLLISION_BOX_INSET = 5;

        //Default Sprite constants
        public const int TREX_DEF_SPRITE_X = 848;
        public const int TREX_DEF_SPRITE_Y = 0;
        public const int TREX_DEF_SPRITE_WIDTH = 44;
        public const int TREX_DEF_SPRITE_HEIGHT = 52;

        //Idle anim constants
        public const int IDLE_BACKGROUND_SPRITE_POS_X = 40;
        public const int IDLE_BACKGROUND_SPRITE_POS_Y = 0;
        private const float BLINK_ANIMATION_RANDOM_MIN = 2f;
        private const float BLINK_ANIMATION_RANDOM_MAX = 10f;
        private const float BLINK_ANIMATION_EYE_CLOSE_TIME = 0.2f;

        //Running anim constants
        private const float ANIMATION_FRAME_LENGTH = 0.1f;
        private const int TREX_RUNNING_SPRITE_ONE_POS_X = TREX_DEF_SPRITE_X + TREX_DEF_SPRITE_WIDTH * 2;
        private const int TREX_RUNNING_SPRITE_ONE_POS_Y = 0;

        //Ducking anim constants
        private const int TREX_DUCKING_SPRITE_WIDTH = 59;
        private const int TREX_DUCKING_SPRITE_HEIGHT = 30;
        private const int TREX_DUCKING_SPRITE_ONE_POS_X = TREX_DEF_SPRITE_X + TREX_DEF_SPRITE_WIDTH * 6;
        private const int TREX_DUCKING_SPRITE_ONE_POS_Y = 0;

        //Death Sprite constants
        private const int TREX_DEATH_SPRITE_POS_X = 1068;
        private const int TREX_DEATH_SPRITE_POS_Y = 0;
        

        //Sprites
        private Sprite idleBackgroundSprite;
        private Sprite _idleSprite;
        private Sprite _idleBlinkSprite;
        private Sprite _runningSpriteOne;
        private Sprite _runningSpriteTwo;
        private Sprite _duckSpriteOne;
        private Sprite _duckSpriteTwo;
        private Sprite _deathSprite;

        //Animation groups
        private SpriteAnimation _blinkAnimation;
        private SpriteAnimation _runAnimation;
        private SpriteAnimation _duckAnimation;

        //sfx
        private SoundEffect _jumpSound;


        private GraphicsDevice _gD;
        private Random _random;

        //Trex Physics components
        private float _yVelocity;
        private float _startPosY;
        private float _dropVelocity;
        public float InitialSpeed => START_SPEED;
        public Vector2 Position { get; set; }
        public float Speed { get; private set; }

        public Texture2D ShowHitBox 
        {
            get
            {
                Texture2D temp = new Texture2D(_gD, HitBox.Width, HitBox.Height);
                Color[] data = new Color[HitBox.Width * HitBox.Height];
                for(int i=0; i<data.Length; i++) data[i] = new Color(255,0,0,100);
                temp.SetData(data);
                return temp;
            }
        }

        //Events
        public event EventHandler JumpComplete;
        public event EventHandler Died;

        //Trex State modifiers
        public int DrawOrder {get; set;}

        public TrexState State { get; private set; }

        public bool IsAlive { get; private set; }

        public Rectangle HitBox
        {
            get
            {
                Rectangle box;
                if(State != TrexState.Ducking)
                {
                    box = new Rectangle(
                    (int)Math.Round(Position.X), 
                    (int)Math.Round(Position.Y - COLLISION_BOX_INSET), 
                    TREX_DEF_SPRITE_WIDTH, 
                    TREX_DEF_SPRITE_HEIGHT
                    );
                }
                else
                {
                    box = new Rectangle(
                    (int)Math.Round(Position.X), 
                    (int)Math.Round(Position.Y - COLLISION_BOX_INSET + (TREX_DEF_SPRITE_HEIGHT - TREX_DUCKING_SPRITE_HEIGHT)), 
                    TREX_DUCKING_SPRITE_WIDTH, 
                    TREX_DUCKING_SPRITE_HEIGHT
                    );
                }
                
                box.Inflate(-COLLISION_BOX_INSET, -COLLISION_BOX_INSET);
                return box;
            }
        }

        public Trex(Texture2D spriteSheet, Vector2 position, SoundEffect jumpSound, GraphicsDevice graphicsDevice)
        {

            Position = position;
            idleBackgroundSprite = new Sprite(spriteSheet, IDLE_BACKGROUND_SPRITE_POS_X, IDLE_BACKGROUND_SPRITE_POS_Y, TREX_DEF_SPRITE_WIDTH, TREX_DEF_SPRITE_HEIGHT);
            State = TrexState.Idle;
            _gD = graphicsDevice;

            _jumpSound = jumpSound;

            _random = new Random();

            _idleSprite = new Sprite(
                spriteSheet, 
                TREX_DEF_SPRITE_X, 
                TREX_DEF_SPRITE_Y, 
                TREX_DEF_SPRITE_WIDTH, 
                TREX_DEF_SPRITE_HEIGHT
                );
            _idleBlinkSprite = new Sprite(
                spriteSheet, 
                TREX_DEF_SPRITE_X + TREX_DEF_SPRITE_WIDTH, 
                TREX_DEF_SPRITE_Y, 
                TREX_DEF_SPRITE_WIDTH, 
                TREX_DEF_SPRITE_HEIGHT
                );

            _runningSpriteOne = new Sprite(
                spriteSheet, 
                TREX_RUNNING_SPRITE_ONE_POS_X, 
                TREX_RUNNING_SPRITE_ONE_POS_Y, 
                TREX_DEF_SPRITE_WIDTH, 
                TREX_DEF_SPRITE_HEIGHT
                );
            _runningSpriteTwo = new Sprite(
                spriteSheet, 
                TREX_RUNNING_SPRITE_ONE_POS_X + TREX_DEF_SPRITE_WIDTH, 
                TREX_RUNNING_SPRITE_ONE_POS_Y, 
                TREX_DEF_SPRITE_WIDTH, 
                TREX_DEF_SPRITE_HEIGHT
                );

            _duckSpriteOne = new Sprite(
                spriteSheet, 
                TREX_DUCKING_SPRITE_ONE_POS_X, 
                TREX_DUCKING_SPRITE_ONE_POS_Y, 
                TREX_DUCKING_SPRITE_WIDTH, 
                TREX_DEF_SPRITE_HEIGHT
                );
            _duckSpriteTwo = new Sprite(
                spriteSheet, 
                TREX_DUCKING_SPRITE_ONE_POS_X + TREX_DUCKING_SPRITE_WIDTH, 
                TREX_DUCKING_SPRITE_ONE_POS_Y, 
                TREX_DUCKING_SPRITE_WIDTH, 
                TREX_DEF_SPRITE_HEIGHT
                );

            _deathSprite = new Sprite(
                spriteSheet, 
                TREX_DEATH_SPRITE_POS_X, 
                TREX_DEATH_SPRITE_POS_Y, 
                TREX_DEF_SPRITE_WIDTH, 
                TREX_DEF_SPRITE_HEIGHT
                );

            _blinkAnimation = new SpriteAnimation();
            _runAnimation = new SpriteAnimation();
            _duckAnimation = new SpriteAnimation();
 
            CreateBlinkAnimation();
            CreateRunAnimation();
            CreateDuckingAnimation();

            _startPosY = Position.Y;

            IsAlive = true;

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if(IsAlive)
            {

                if (State == TrexState.Idle)
                {

                    idleBackgroundSprite.Draw(spriteBatch, Position);
                    _blinkAnimation.Draw(spriteBatch, Position);

                }
                else if (State == TrexState.Jumping || State == TrexState.Falling)
                {
                    
                    _idleSprite.Draw(spriteBatch, Position);

                }
                else if (State == TrexState.Running)
                {

                    _runAnimation.Draw(spriteBatch, Position);

                }
                else if (State == TrexState.Ducking)
                {

                    _duckAnimation.Draw(spriteBatch, Position);

                }
            }
            else
            {
                _deathSprite.Draw(spriteBatch, Position);
            }
            //spriteBatch.Draw(ShowHitBox, new Vector2(HitBox.X, HitBox.Y), Color.White);

        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (State == TrexState.Idle)
            { 

                if(!_blinkAnimation.IsPlaying)
                {
                    CreateBlinkAnimation();
                    _blinkAnimation.Play();
                }

                _blinkAnimation.Update(gameTime);

            }
            else if (State == TrexState.Jumping || State == TrexState.Falling)
            {

                Position = new Vector2(Position.X, Position.Y + (_yVelocity + _dropVelocity) * deltaTime);
                _yVelocity += GRAVITY * deltaTime;

                if (_yVelocity >= 0)
                    State = TrexState.Falling;
                

                if(Position.Y >= _startPosY)
                {

                    Position = new Vector2(Position.X, _startPosY);
                    _yVelocity = 0;
                    State = TrexState.Running;

                    OnJumpComplete();

                }

            }
            else if (State == TrexState.Running)
            {
                if(!_runAnimation.IsPlaying)
                {
                    _runAnimation.Play();
                }

                if(Speed < MAX_SPEED)
                    Speed += SPEED_ACCELERATION * deltaTime;
                    

                _runAnimation.Update(gameTime);

                

            }
            else if (State == TrexState.Ducking)
            {

                if(!_duckAnimation.IsPlaying)
                {
                    _duckAnimation.Play();
                }

                _duckAnimation.Update(gameTime);

            }

            _dropVelocity = 0;

        }

        public void Initialize()
        {

            Speed = START_SPEED;
            State = TrexState.Running;
            IsAlive = true;
            Position = new Vector2(Position.X, _startPosY);

        }

        private void CreateBlinkAnimation()
        {
            
            _blinkAnimation.Clear();
            _blinkAnimation.ShouldLoop = false;
            double blinkTimeStamp = BLINK_ANIMATION_RANDOM_MIN + _random.NextDouble() * (BLINK_ANIMATION_RANDOM_MAX - BLINK_ANIMATION_RANDOM_MIN);
            _blinkAnimation.AddFrame(_idleSprite, 0);
            _blinkAnimation.AddFrame(_idleBlinkSprite, (float)blinkTimeStamp);
            _blinkAnimation.AddFrame(_idleSprite, (float)blinkTimeStamp + BLINK_ANIMATION_EYE_CLOSE_TIME);
            
        }

        public bool BeginJump()
        {

            if(State == TrexState.Jumping || State == TrexState.Falling)
                return false;
        
            _jumpSound.Play();

            State = TrexState.Jumping;

            _yVelocity = JUMP_START_VELOCITY;

            return true;
            
        }

        public bool CancelJump()
        {

            if (State != TrexState.Jumping || (_startPosY -Position.Y) < MIN_JUMP_HEIGHT)
                return false;

            _yVelocity = _yVelocity < CANCEL_JUMP_VELOCITY ? CANCEL_JUMP_VELOCITY : 0;

            return true;

        }

        public bool Duck()
        {

            if(State == TrexState.Jumping || State == TrexState.Falling)
                return false;
            
            State = TrexState.Ducking;

            return true;

        }

        public bool GetUp()
        {

            if(State != TrexState.Ducking)
                return false;
            State = TrexState.Running;
            return true;

        }

        public bool Drop()
        {
            
            if(State != TrexState.Falling && State != TrexState.Jumping)
                return false;

            State = TrexState.Falling;
            _dropVelocity = DROP_VELOCITY;
            return true;
            
        }

        public void CreateRunAnimation()
        {

            _runAnimation.AddFrame(_runningSpriteOne, 0);
            _runAnimation.AddFrame(_runningSpriteTwo, ANIMATION_FRAME_LENGTH);
            _runAnimation.AddFrame(_runningSpriteOne, ANIMATION_FRAME_LENGTH * 2);

        }

        public void CreateDuckingAnimation()
        {

            _duckAnimation.AddFrame(_duckSpriteOne,0);
            _duckAnimation.AddFrame(_duckSpriteTwo, ANIMATION_FRAME_LENGTH);
            _duckAnimation.AddFrame(_duckSpriteOne, ANIMATION_FRAME_LENGTH * 2);

        }

        protected virtual void OnJumpComplete()
        {

            EventHandler handler = JumpComplete;
            handler?.Invoke(this, EventArgs.Empty);

        }

        protected virtual void OnDied()
        {
            EventHandler handler = Died;
            handler?.Invoke(this, EventArgs.Empty);
        }

        public bool Die()
        {
            if(!IsAlive)
                return false;

            State = TrexState.Idle;
            Speed = 0;

            IsAlive = false;

            OnDied();

            return true;
        }
        
    }
}
