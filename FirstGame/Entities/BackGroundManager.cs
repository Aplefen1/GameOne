using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using FirstGame.Graphics;
using Microsoft.Xna.Framework.Audio;
using FirstGame;
using System.Linq;

namespace FirstGame.Entities
{

    public class BackGroundManager : IGameEntity
    {
        private const int CLOUD_WIDTH = 48;
        private const int CLOUD_HEIGHT = 16;
        private const int CLOUD_SPRITE_COORDS_X = 85;
        private const int CLOUD_SPRITE_COORDS_Y = 0;

        private const int MIN_CLOUD_Y = 0;
        private const int MAX_CLOUD_Y = 80;
        private const int MAX_DIST_CLOUDS = 500;
        private const int MIN_DIST_CLOUDS = 50;
        private const int WINDOW_WIDTH = TrexRunner.WINDOW_WIDTH;
        private const float CLOUD_RELATIVE_SPEED = 0.2f;
        private Trex _trex;
        private EntityManager _entityManager;
        private Random _random;
        private Texture2D _spriteSheet;
        private List<BackGroundObject> _skyObjects = new List<BackGroundObject>();
        private Sprite _cloudSprite;
        private float _lastSpawnPosition;

        public int DrawOrder { get; set; }
        public bool IsEnabled { get; set; }

        public BackGroundManager(Texture2D spriteSheet, EntityManager entityManager, Trex trex)
        {
            _spriteSheet = spriteSheet;
            _entityManager = entityManager;
            _trex = trex;
            _random = new Random();

            _cloudSprite = new Sprite(_spriteSheet, CLOUD_SPRITE_COORDS_X, CLOUD_SPRITE_COORDS_Y, CLOUD_WIDTH, CLOUD_HEIGHT);
            
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            
        }

        public void Update(GameTime gameTime)
        {
            

            if ( _skyObjects.Any() )
            {
                _lastSpawnPosition =_skyObjects.Max(f => f.PositionX);

                bool shouldSpawn = IsEnabled && _lastSpawnPosition > WINDOW_WIDTH && _lastSpawnPosition < WINDOW_WIDTH;

                if (shouldSpawn)
                {
                    SpawnCloud();
                    Console.WriteLine("NewCloud");
                }

                foreach(BackGroundObject o in _skyObjects)
                {

                    if(o.PositionX < -CLOUD_WIDTH)
                    {
                        _entityManager.RemoveEntity(o);
                    }
                }

            }

        }

        public void SpawnCloud()
        {
            BackGroundObject skyObject;

            int posX = WINDOW_WIDTH + _random.Next(MIN_DIST_CLOUDS,MAX_DIST_CLOUDS);
            int posY = _random.Next(MIN_CLOUD_Y, MAX_CLOUD_Y);

            float speed = (float)_random.NextDouble() * 0.5f;

            skyObject = new BackGroundObject(posX, posY, _cloudSprite, speed, _trex);
            skyObject.DrawOrder = 0;
            _skyObjects.Add(skyObject);
            _entityManager.AddEntity(skyObject);
            
        }

        public void Initilaize()
        {
            _skyObjects.Clear();
            SpawnCloud();
            IsEnabled = true;

        }
        public void Stop()
        {
            IsEnabled = false;
            foreach(BackGroundObject o in _skyObjects)
            {
                _entityManager.RemoveEntity(o);
            }
        }

        
    }
}