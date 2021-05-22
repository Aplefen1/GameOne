using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FirstGame.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace FirstGame.Entities
{

    public interface ICollidable
    {
        Rectangle HitBox{ get; }
        Texture2D ShowHitBox { get; }
    }
}