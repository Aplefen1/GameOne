using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using FirstGame.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace FirstGame.Entities
{

    public class ObstacleManager : IGameEntity
    {
        private static readonly int[] FLYING_DINO_Y_POSITIONS = new int[] { 90, 60, 30 };

        private const float MIN_SPAWN_DISTANCE = 10;
        private const int MIN_OBSTACLE_DISTANCE = 5;
        private const int MAX_OBSTACLE_DISTANCE = 25;
        private const int MAX_DINO_HEIGHT = TrexRunner.WINDOW_HEIGHT - FlyingDino.DINO_HEIGHT;
        private const int MIN_DINO_HEIGHT = 0;
        private const int SPAWN_Y_LEVEL = 95;
        private const int OBSTACLE_DRAW_ORDER = 12;

        private const int FLYING_DINO_SCORE_MIN = 100;
        private const float OBSTACLE_DISTANCE_SHRINKER = 1.5f;

        private float _minObstacleDistance => (_trex.Speed > _trex.InitialSpeed * OBSTACLE_DISTANCE_SHRINKER) ? (MIN_OBSTACLE_DISTANCE * OBSTACLE_DISTANCE_SHRINKER) : (MIN_OBSTACLE_DISTANCE * (_trex.Speed / _trex.InitialSpeed));
        private float _maxObstacleDistance => MAX_OBSTACLE_DISTANCE * ((_trex.InitialSpeed+100) / _trex.Speed );

        private readonly EntityManager _entityManager;
        private readonly Trex _trex;
        private readonly ScoreBoard _scoreBoard;
        private Random _random;
        private Texture2D _spriteSheet;

        public bool IsEnabled { get; set; } = true;

        public bool CanSpawnObstacles => IsEnabled && _scoreBoard.Score >= MIN_SPAWN_DISTANCE;

        private double _lastSpawnScore = -1;
        private double _currentTargetDistance;
        private GraphicsDevice _gd;

        public int DrawOrder => 0;

        public ObstacleManager(EntityManager entityManager, Trex trex, ScoreBoard scoreBoard, Texture2D spriteSheet, GraphicsDevice graphicsDevice)
        {
 
            _entityManager = entityManager;
            _trex = trex;
            _scoreBoard = scoreBoard;
            _random = new Random();
            _spriteSheet = spriteSheet;
            _gd = graphicsDevice;

        }

        public void Update(GameTime gameTime)
        {

            if(!IsEnabled)
                return;

            if(CanSpawnObstacles && (
                _lastSpawnScore <= 0 || (
                _scoreBoard.Score - _lastSpawnScore >= _currentTargetDistance
                )))
            {
                _currentTargetDistance = _random.NextDouble() 
                    * (_maxObstacleDistance - _minObstacleDistance) + _minObstacleDistance;

                _lastSpawnScore = _scoreBoard.Score;
                Console.WriteLine(_maxObstacleDistance);
                
                SpawnRandomObstacle();
            }

            foreach(Obstacle obstacle in _entityManager.GetEntitiesOfType<Obstacle>())
            {
                if(obstacle.Position.X < -100)
                {

                    _entityManager.RemoveEntity(obstacle);

                }
            }

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {


        }

        private void SpawnRandomObstacle()
        {
            //TODO: Object instance creation, add to em

            Obstacle obstacle = null; 
            float cactusGroupSpawnRate = 0.7f;
            float dinoSpawnRate = _scoreBoard.Score >= FLYING_DINO_SCORE_MIN ? 0.25f : 0;

            float spawnGroup = (float)_random.NextDouble();

            if (spawnGroup <= cactusGroupSpawnRate || dinoSpawnRate == 0)
            {

                CactusGroup.GroupSize randomGroupSize = (CactusGroup.GroupSize)_random.Next((int)CactusGroup.GroupSize.Small, (int)CactusGroup.GroupSize.Large + 1);
                bool cactusType = _random.NextDouble() > 0.5;
                
                obstacle = new CactusGroup(_spriteSheet, cactusType, randomGroupSize, _trex, new Vector2(TrexRunner.WINDOW_WIDTH, SPAWN_Y_LEVEL), _gd);

            }
            else if(spawnGroup > cactusGroupSpawnRate && spawnGroup < 1f)
            {
                int vertialPosIndex = _random.Next(0, FLYING_DINO_Y_POSITIONS.Length);
                float posY = FLYING_DINO_Y_POSITIONS[vertialPosIndex];

                obstacle = new FlyingDino(_trex, new Vector2(TrexRunner.WINDOW_WIDTH, posY), _spriteSheet);
            }

            obstacle.DrawOrder = OBSTACLE_DRAW_ORDER;

            _entityManager.AddEntity(obstacle);
        }

        public void Reset()
        {

            foreach(Obstacle obstacle in _entityManager.GetEntitiesOfType<Obstacle>())
            {
                _entityManager.RemoveEntity(obstacle);
            }
            _lastSpawnScore = 0;
        }
    }
}
