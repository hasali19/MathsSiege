using MathsSiege.Client.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace MathsSiege.Client
{
    public class EnemySpawner
    {
        private GameMap gameMap;
        private EnemyManager enemyManager;

        private Random random = new Random();

        private TimeSpan elapsedTime;
        private DateTime nextSpawnTime;

        public EnemySpawner(GameMap gameMap, EnemyManager enemyManager)
        {
            this.gameMap = gameMap;
            this.enemyManager = enemyManager;
            nextSpawnTime = DateTime.Now.AddMilliseconds(GetNextDelay());
        }

        public void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            // Check if enough time has passed.
            if (DateTime.Now >= nextSpawnTime)
            {
                // Call the registered spawn action.
                SpawnEnemy();

                // Reset the next spawn time.
                var nextDelay = GetNextDelay();
                nextSpawnTime = DateTime.Now.AddMilliseconds(nextDelay);
            }
        }

        /// <summary>
        /// Spawns a random enemy type on a random tile.
        /// </summary>
        private void SpawnEnemy()
        {
            var i = random.Next(gameMap.SpawnableTiles.Count);
            var tile = gameMap.SpawnableTiles[i];
            enemyManager.CreateRandomEnemy(tile);
        }

        /// <summary>
        /// Gets the number of milliseconds to wait until the next spawn.
        /// </summary>
        /// <returns></returns>
        private double GetNextDelay()
        {
            var elapsedTimeNormalized = (float)Math.Floor(elapsedTime.TotalMinutes / 0.5);
            return MathHelper.Max(15_000 - elapsedTimeNormalized * 500, 5_000);
        }
    }
}
