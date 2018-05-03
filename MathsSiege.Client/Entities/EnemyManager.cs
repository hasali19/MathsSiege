using MathsSiege.Client.Content.DataTypes;
using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Collections;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MathsSiege.Client.Entities
{
    public enum EnemyType
    {
        Wyvern = 0,
        Skeleton = 1,
        Goblin = 2
    }

    public class EnemyManager : DrawableEntity
    {
        private Random random = new Random();

        private Bag<Enemy> enemies = new Bag<Enemy>(50);

        private SpriteSheetAnimationFactory goblinAnimationFactory;
        private SpriteSheetAnimationFactory skeletonAnimationFactory;
        private SpriteSheetAnimationFactory wyvernAnimationFactory;

        /// <summary>
        /// Creates a new enemy.
        /// </summary>
        /// <param name="type">The type of enemy to create.</param>
        /// <param name="tile">The tile on which to place the enemy.</param>
        /// <returns>True if the enemy was created successfully.</returns>
        public bool CreateEnemy(EnemyType type, Tile tile)
        {
            if (!tile.IsSpawnable)
            {
                return false;
            }

            Enemy enemy;

            switch (type)
            {
                case EnemyType.Goblin:
                    enemy = new Enemy(goblinAnimationFactory)
                    {
                        MovementSpeedMultiplier = 1.2f,
                        AttackDamage = 10,
                        AttackInterval = 1000
                    };
                    break;

                case EnemyType.Skeleton:
                    enemy = new Enemy(skeletonAnimationFactory)
                    {
                        MovementSpeedMultiplier = 0.7f,
                        AttackDamage = 20,
                        AttackInterval = 2000,
                        MaxHealth = 150
                    };
                    break;

                case EnemyType.Wyvern:
                    enemy = new Enemy(wyvernAnimationFactory)
                    {
                        Origin = new Vector2(128, 192),
                        Scale = new Vector2(0.625f),
                        MovementSpeedMultiplier = 1.0f,
                        AttackDamage = 30,
                        AttackInterval = 2000,
                        MaxHealth = 200,
                        IsFlying = true
                    };
                    break;

                default:
                    return false;
            }

            enemy.Scene = Scene;
            enemy.Position = tile.Position + new Vector2(0.5f);

            enemies.Add(enemy);
            enemy.OnAddedToScene();

            enemy.Destroyed += (attackable) => enemies.Remove(enemy);

            return true;
        }

        /// <summary>
        /// Creates a new enemy of a random type.
        /// </summary>
        /// <param name="tile">The tile on which to place the enemy.</param>
        /// <returns>True if the enemy was created successfully.</returns>
        public bool CreateRandomEnemy(Tile tile)
        {
            var typeValues = typeof(EnemyType).GetEnumValues();
            var type = null as EnemyType?;

            // Generate a random number between 0 and 1.
            var r = random.NextDouble();

            // Calculate the sum of the values for each type,
            // e.g. For values of Goblin = 3, Skeleton = 2, Wyvern = 1,
            // the sum is 3 + 2 + 1 = 6.
            var max = (typeValues.Length / 2d) * (typeValues.Length + 1);

            // Loop through all the enemy types.
            for (int i = 0; i < typeValues.Length; i++)
            {
                // Check if the generated number is less than
                // the threshold required to spawn that enemy.
                // e.g. For values of Goblin = 3, Skeleton = 2, Wyvern = 1,
                // the threshold for spawning a wyvern would be 1 / 6,
                // while for a skeleton it would be (2 + 1) / 6 = 1 / 2.
                if (r <= (((i + 1) / 2d) * (i + 2)) / max)
                {
                    type = (EnemyType)typeValues.GetValue(i);
                    break;
                }
            }

            return CreateEnemy(type.Value, tile);
        }

        public override void OnAddedToScene()
        {
            var goblinAnimations = Scene.Content.Load<Animation[]>(ContentPaths.AnimatedSprites.GoblinAnimations);
            var skeletonAnimations = Scene.Content.Load<Animation[]>(ContentPaths.AnimatedSprites.SkeletonAnimations);
            var wyvernAnimations = Scene.Content.Load<Animation[]>(ContentPaths.AnimatedSprites.WyvernAnimations);

            var goblinAtlas = Scene.Content.Load<TextureAtlas>(ContentPaths.AnimatedSprites.GoblinAtlas);
            var skeletonAtlas = Scene.Content.Load<TextureAtlas>(ContentPaths.AnimatedSprites.SkeletonAtlas);
            var wyvernAtlas = Scene.Content.Load<TextureAtlas>(ContentPaths.AnimatedSprites.WyvernAtlas);

            goblinAnimationFactory = LoadAnimationFactory(goblinAtlas, goblinAnimations);
            skeletonAnimationFactory = LoadAnimationFactory(skeletonAtlas, skeletonAnimations);
            wyvernAnimationFactory = LoadAnimationFactory(wyvernAtlas, wyvernAnimations);
        }

        /// <summary>
        /// Checks if a tile currently contains an enemy.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public bool CheckTileContainsEnemy(Tile tile)
        {
            var area = new Rectangle(tile.X, tile.Y, 1, 1);

            foreach (var enemy in enemies)
            {
                if (area.Contains(enemy.Position))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the nearest enemy within range of a particular position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public Enemy GetNearestEnemyInRange(Vector2 position, float range)
        {
            Enemy nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (var enemy in enemies)
            {
                float distance = (enemy.Position - position).Length();
                if (distance < range && (nearest == null || distance < nearestDistance))
                {
                    nearest = enemy;
                    nearestDistance = distance;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Gets all the enemies within range of a position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public IReadOnlyCollection<Enemy> GetEnemiesInRange(Vector2 position, float range)
        {
            var enemies = new List<Enemy>();

            foreach (var enemy in this.enemies)
            {
                var distance = (enemy.Position - position).Length();
                if (distance < range)
                {
                    enemies.Add(enemy);
                }
            }

            return enemies;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var enemy in enemies)
            {
                enemy.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var enemy in enemies)
            {
                enemy.Draw(gameTime);
            }
        }

        private SpriteSheetAnimationFactory LoadAnimationFactory(TextureAtlas atlas, Animation[] animations)
        {
            var animationFactory = new SpriteSheetAnimationFactory(atlas);

            foreach (var animation in animations)
            {
                animationFactory.Add(animation.Name, new SpriteSheetAnimationData(animation.FrameIndices,
                    animation.FrameDuration, animation.IsLooping));
            }

            return animationFactory;
        }
    }
}
