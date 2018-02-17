using MathsSiege.Client.Content.DataTypes;
using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Collections;
using MonoGame.Extended.TextureAtlases;
using System;

namespace MathsSiege.Client.Entities
{
    public enum EnemyType
    {
        Goblin,
        Skeleton
    }

    public class EnemyManager : DrawableEntity
    {
        private Random random = new Random();

        private Bag<Enemy> enemies = new Bag<Enemy>(50);

        private SpriteSheetAnimationFactory goblinAnimationFactory;
        private SpriteSheetAnimationFactory skeletonAnimationFactory;

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
                    enemy = new Enemy(this.goblinAnimationFactory);
                    break;

                case EnemyType.Skeleton:
                    enemy = new Enemy(this.skeletonAnimationFactory);
                    break;

                default:
                    return false;
            }

            enemy.Scene = this.Scene;
            enemy.Position = tile.Position + new Vector2(0.5f);

            this.enemies.Add(enemy);
            enemy.OnAddedToScene();

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
            var typeIndex = this.random.Next(typeValues.Length);
            var type = (EnemyType)typeValues.GetValue(typeIndex);

            return this.CreateEnemy(type, tile);
        }

        public override void OnAddedToScene()
        {
            var goblinAnimations = this.Scene.Content.Load<Animation[]>(ContentPaths.AnimatedSprites.GoblinAnimations);
            var skeletonAnimations = this.Scene.Content.Load<Animation[]>(ContentPaths.AnimatedSprites.SkeletonAnimations);

            var goblinAtlas = this.Scene.Content.Load<TextureAtlas>(ContentPaths.AnimatedSprites.GoblinAtlas);
            var skeletonAtlas = this.Scene.Content.Load<TextureAtlas>(ContentPaths.AnimatedSprites.SkeletonAtlas);

            this.goblinAnimationFactory = this.LoadAnimationFactory(goblinAtlas, goblinAnimations);
            this.skeletonAnimationFactory = this.LoadAnimationFactory(skeletonAtlas, skeletonAnimations);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var enemy in this.enemies)
            {
                enemy.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var enemy in this.enemies)
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
