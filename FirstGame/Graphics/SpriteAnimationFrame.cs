using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace FirstGame.Graphics
{
    public class SpriteAnimationFrame
    {
        private Sprite _sprite;
        public Sprite Sprite 
        { 
            get
            {
                return _sprite;
            } 
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "The sprite cant be null");
                }
                else
                {
                    _sprite = value;
                }
            } 
        }

        public float TimeStamp { get; }

        public SpriteAnimationFrame(Sprite sprite, float timeStamp)
        {
            Sprite = sprite;
            TimeStamp = timeStamp;
        }
        
    }
}
