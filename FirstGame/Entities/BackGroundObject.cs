using System.Collections.Generic;
using System;
using FirstGame.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstGame.Entities
{

    public class BackGroundObject : IGameEntity
    {
        private float _positionY;
        public float PositionX { get; set; }
        public Sprite Sprite { get; set; }
        public int DrawOrder { get; set; }
        public float Speed { get; set; }
        private Trex _trex;
        public BackGroundObject(float positionX, float positionY, Sprite sprite, float speed, Trex trex) 
        {
            Sprite = sprite;
            PositionX = positionX;
            _positionY = positionY;
            Speed = speed;
            _trex = trex;

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Sprite.Draw(spriteBatch, new Vector2(PositionX, _positionY));
        }

        public void Update(GameTime gameTime)
        {
            PositionX -= _trex.Speed * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}