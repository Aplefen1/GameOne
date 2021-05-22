using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using FirstGame.Graphics;

namespace FirstGame.Entities
{

    public class CactusGroup : Obstacle
    {
        private const int SMALL_CACTUS_SPRITE_WIDTH = 17;
        private const int SMALL_CACTUS_SPRITE_HEIGHT = 36;
        private const int SMALL_CACTUS_TEXTURE_POS_X = 228;
        private const int SMALL_CACTUS_TEXTURE_POS_Y = 0;

        private const int BIG_CACTUS_SPRITE_WIDTH = 25;
        private const int BIG_CACTUS_SPRITE_HEIGHT = 51;
        private const int BIG_CACTUS_TEXTURE_POS_X = 332;
        private const int BIG_CACTUS_TEXTURE_POS_Y = 0;

        private const int COLLISION_BOX_INSET = 3;
        

        public enum GroupSize
        {
            Small = 0,
            Medium = 1,
            Large = 2
        }

        public override Rectangle HitBox
        {
            get
            {
                Rectangle box = new Rectangle((int)Math.Round(Position.X), (int)Math.Round(Position.Y+COLLISION_BOX_INSET), Sprite.Width, Sprite.Height);
                box.Inflate(-COLLISION_BOX_INSET, -COLLISION_BOX_INSET);
                return box;
            }

        }

        public override Texture2D ShowHitBox 
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

        public bool IsLarge { get; private set; }

        public GroupSize Size { get; }
        public Sprite Sprite { get; set; }
        private GraphicsDevice _gD;

        public CactusGroup(Texture2D spriteSheet, bool isLarge, GroupSize size, Trex trex, Vector2 position, GraphicsDevice graphicsDevice) : base(trex, position)
        {

            IsLarge = isLarge;
            Size = size;
            Sprite = GenerateSprite(spriteSheet);
            HitBox.Inflate(-3, -3);
            _gD = graphicsDevice;
            Speed = 1;

        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, Position);
            //spriteBatch.Draw(ShowHitBox, new Vector2(HitBox.X, HitBox.Y), Color.White);
        }

        public Sprite GenerateSprite(Texture2D spriteSheet)
        {

            Sprite sprite = null;

            int spriteWidth = 0;
            int spriteHeight = 0;
            int posX = 0;
            int posY = 0;
            int bigYOffset = BIG_CACTUS_SPRITE_HEIGHT - SMALL_CACTUS_SPRITE_HEIGHT;

            if(IsLarge) //create Large
            {

                spriteWidth = BIG_CACTUS_SPRITE_WIDTH;
                spriteHeight = BIG_CACTUS_SPRITE_HEIGHT;

                posX = BIG_CACTUS_TEXTURE_POS_X;
                posY = BIG_CACTUS_TEXTURE_POS_Y;
                Position -= new Vector2(0,bigYOffset);

            }
            else //create smol
            {

                spriteWidth = SMALL_CACTUS_SPRITE_WIDTH;
                spriteHeight = SMALL_CACTUS_SPRITE_HEIGHT;

                posX = SMALL_CACTUS_TEXTURE_POS_X;
                posY = SMALL_CACTUS_TEXTURE_POS_Y;

            }
            
            int offsetX = 0;
            int width = spriteWidth;

            if(Size == GroupSize.Small)
            {
                offsetX = 0;
                width = spriteWidth;
            }
            else if (Size == GroupSize.Medium)
            {
                offsetX = 1;
                width = spriteWidth * 2;
            }
            else
            {
                offsetX = 3;
                width = spriteWidth * 3;

            }

            sprite = new Sprite
            (
                spriteSheet, 
                posX + offsetX * spriteWidth, //posx
                posY, //posy
                width, //width
                spriteHeight //height
            );
        

            return sprite;

        }


    }
}