using System.Collections.Generic;
using System;
using FirstGame.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace FirstGame.Entities
{
    public class GroundManager : IGameEntity
    {
        private const int GROUND_SPRITE_X = 02;
        private const int GROUND_SPRITE_Y = 54;
        private const int GROUND_SPRITE_WIDTH = 600;
        private const int GROUND_SPRITE_HEIGHT = 14;
        private const int GROUND_TILE_POS_Y = 119;

        public readonly List<GroundTile> _groundSprites;
        public Vector2 Position { get; set; }
        public int DrawOrder{ get; }
        private readonly EntityManager _entityManager;

        private Texture2D _spriteSheet;

        private Sprite _regularSprite;
        private Sprite _bumpySprite;
        private Trex _trex;
        private Random _random;
        
        public GroundManager(Texture2D spriteSheet, EntityManager entityManager, Trex trex)
        {

            _spriteSheet = spriteSheet;
            _entityManager = entityManager;
            _groundSprites = new List<GroundTile>();
            _trex = trex;
            _random = new Random();

            _regularSprite = new Sprite(spriteSheet, GROUND_SPRITE_X, GROUND_SPRITE_Y, GROUND_SPRITE_WIDTH, GROUND_SPRITE_HEIGHT);
            _bumpySprite = new Sprite(spriteSheet, GROUND_SPRITE_X + GROUND_SPRITE_WIDTH, GROUND_SPRITE_Y, GROUND_SPRITE_WIDTH, GROUND_SPRITE_HEIGHT);

        }

        public void Update(GameTime gameTime)
        {
            
            if(_groundSprites.Any() && _groundSprites.Max(g => g.PositionX) < 0)
            {

                SpawnTile();

            }

            List<GroundTile> tilesToRemove = new List<GroundTile>();

            foreach(GroundTile gt in _groundSprites)
            {

                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                gt.PositionX -= _trex.Speed * (float)deltaTime;

                if(gt.PositionX < -GROUND_SPRITE_WIDTH)
                {

                    _entityManager.RemoveEntity(gt);
                    tilesToRemove.Add(gt);

                }
                
            }

            foreach(GroundTile gt in tilesToRemove)
                _groundSprites.Remove(gt);

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {



        }

        public void Initialize()
        {

            _groundSprites.Clear();

            foreach(GroundTile gt in _entityManager.GetEntitiesOfType<GroundTile>())
            {
                _entityManager.RemoveEntity(gt);
            }

            GroundTile groundTile = CreateRegularTile(0);
            _groundSprites.Add(groundTile);

            _entityManager.AddEntity(groundTile);

        }

        private GroundTile CreateRegularTile(float positionX)
        {

            GroundTile groundTile = new GroundTile(_regularSprite, positionX, GROUND_TILE_POS_Y);
            return groundTile;

        }

        private GroundTile CreateBumpyTile(float positionX)
        {

            GroundTile groundTile = new GroundTile(_bumpySprite, positionX, GROUND_TILE_POS_Y);
            return groundTile;
            
        }

        private void SpawnTile()
        {

            //Console.WriteLine("Created tile");
            double randomNumber = _random.NextDouble();
            float tileOffset = _groundSprites.Max(g => g.PositionX);

            GroundTile groundTile;

            if(randomNumber > 0.5)
                groundTile = CreateBumpyTile(GROUND_SPRITE_WIDTH + tileOffset);
            else
                groundTile = CreateRegularTile(GROUND_SPRITE_WIDTH + tileOffset);

            _groundSprites.Add(groundTile);
            _entityManager.AddEntity(groundTile);

        }

    }
}