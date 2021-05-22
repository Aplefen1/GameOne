using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using FirstGame.Graphics;

namespace FirstGame.Entities
{

    public class FlyingDino : Obstacle
    {
        private const int DINO_TEXTURE_COORDS_X = 134;
        private const int DINO_TEXTURE_COORDS_Y = 0;
        private const int DINO_WIDTH = 46;
        public const int DINO_HEIGHT = 42;

        private const int HIT_BOX_INSET = 6;

        private const float FRAME_LENGTH = 0.2f;
        private const int EXTRA_SPEED = 60;
        private SpriteAnimation _animation;

        public override Rectangle HitBox 
        { 
            get
            {
                Rectangle collisionBox = new Rectangle(Position.ToPoint(), new Point(DINO_WIDTH, DINO_HEIGHT));
                collisionBox.Inflate(-HIT_BOX_INSET, -HIT_BOX_INSET);
                return collisionBox;
            }
         }

        public override Texture2D ShowHitBox { get; }

        public FlyingDino(Trex trex, Vector2 position, Texture2D spriteSheet) : base(trex, position)
        {
            CreateAnimation(spriteSheet);
            Speed = 1;

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(_trex.IsAlive)
            {
                _animation.Update(gameTime);
                Position = new Vector2(Position.X - EXTRA_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds, Position.Y);
            }

        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            _animation.Draw(spriteBatch, Position);
            
        }

        private void CreateAnimation(Texture2D spriteSheet)
        {

            Sprite spriteA = new Sprite(spriteSheet, DINO_TEXTURE_COORDS_X, DINO_TEXTURE_COORDS_Y, DINO_WIDTH, DINO_HEIGHT);
            Sprite spriteB = new Sprite(spriteSheet, DINO_TEXTURE_COORDS_X + DINO_WIDTH, DINO_TEXTURE_COORDS_Y, DINO_WIDTH, DINO_HEIGHT);

            _animation = new SpriteAnimation();

            _animation.AddFrame(spriteA, 0);
            _animation.AddFrame(spriteB, FRAME_LENGTH);
            _animation.AddFrame(spriteA, FRAME_LENGTH*2);

            _animation.ShouldLoop = true;
            _animation.Play();

        }

  
    }
}
