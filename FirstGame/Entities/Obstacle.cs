using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using FirstGame.Graphics;

namespace FirstGame.Entities
{

    public abstract class Obstacle : IGameEntity, ICollidable
    {

        protected Trex _trex;

        public abstract Rectangle HitBox { get; }
        public abstract Texture2D ShowHitBox { get; }

        public Vector2 Position { get; set; }

        public float Speed { get; protected set; }

        public int DrawOrder{ get; set; }

        protected Obstacle(Trex trex, Vector2 position)
        {

            _trex = trex;
            Position = position;

        }

        public virtual void Update(GameTime gameTime)
        {

            float deltaTime =  (float)gameTime.ElapsedGameTime.TotalSeconds;

            float posX = _trex.Speed * deltaTime * Speed;

            Position -= new Vector2(posX, 0);

            CheckCollisions();

        }

        public abstract void Draw(SpriteBatch spriteBatch, GameTime gameTime);

        private void CheckCollisions()
        {
            
            Rectangle obstacleCollisionBox = HitBox;
            Rectangle trexCollisionBox = _trex.HitBox;

            if(obstacleCollisionBox.Intersects(trexCollisionBox) && _trex.IsAlive)
            {
                _trex.Die();
            }
        }
    }  
}