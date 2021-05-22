using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FirstGame.Graphics
{
    public class Sprite
    {

        public Texture2D Texture { get; private set; }
        public int X {get; set;}
        public int Y {get; set;}
        public int Width {get; set;}
        public int Height {get; set;}
        public Color TintColor = Color.White;
        
        public Sprite(Texture2D texture, int x, int y, int width, int height, Color? tintColor = null)
        {
            Texture = texture;
            X = x;
            Y = y;
            Width = width;
            Height = height;

            if (!(tintColor is null))
            {
                TintColor = (Color)tintColor;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(Texture, position, new Rectangle(X, Y, Width, Height), TintColor);
        }
    }
}