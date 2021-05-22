using System.Collections.Generic;
using System;
using FirstGame.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstGame.Entities
{

    public class GroundTile : IGameEntity
    {

        private float _positionY;

        public int DrawOrder{ get; }

        public Sprite Sprite { get; set; }

        public float PositionX;

        public GroundTile(Sprite sprite, float positionX, float positionY)
        {

            Sprite = sprite;
            PositionX = positionX;
            _positionY = positionY;
            
        }

         public void Update(GameTime gameTime)
        {


        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {

            Sprite.Draw(spriteBatch, new Vector2(PositionX, _positionY));

        }

        
    }
    
}