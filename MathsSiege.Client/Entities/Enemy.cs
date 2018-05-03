using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MathsSiege.Client.Entities
{
    public enum EnemyState
    {
        Idle,
        Moving,
        Attacking
    }

    public class Enemy : AttackableEntity
    {
        /// <summary>
        /// The tolerance level for vector equality comparisons.
        /// </summary>
        private const float VectorEqualityTolerance = 0.1f;

        /// <summary>
        /// The time to wait for between attacks, in milliseconds.
        /// </summary>
        public int AttackInterval { get; set; } = 1500;

        /// <summary>
        /// The multiplier for the enemy's movement speed.
        /// </summary>
        public float MovementSpeedMultiplier { get; set; } = 1f;

        /// <summary>
        /// The damage dealt by each attack.
        /// </summary>
        public int AttackDamage { get; set; } = 10;

        /// <summary>
        /// The position of the enemy on the map.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// Sets whether the enemy can fly over defences.
        /// </summary>
        public bool IsFlying { get; set; }

        /// <summary>
        /// Sets the sprite texture origin.
        /// </summary>
        public Vector2 Origin
        {
            get => sprite.Origin;
            set => sprite.Origin = value;
        }

        public Vector2 Scale
        {
            get => sprite.Scale;
            set => sprite.Scale = value;
        }

        /// <summary>
        /// The state of the enemy AI.
        /// </summary>
        public EnemyState State { get; private set; }

        /// <summary>
        /// The direction in which the enemy is facing.
        /// </summary>
        public Direction Facing { get; private set; }

        private GameMap map;
        private WallManager wallManager;
        private DefenceManager defenceManager;
        private Castle castle;

        private AnimatedSprite sprite;

        private SoundEffect swordAttackSound;

        private IEnemyTarget target;
        private IList<Tile> path;

        private Vector2 velocity;

        private Stopwatch stopwatch = new Stopwatch();

        private Vector2 TargetTilePosition => path.Last().Position + new Vector2(0.5f);

        public Enemy(SpriteSheetAnimationFactory animationFactory)
        {
            sprite = new AnimatedSprite(animationFactory, "IdleDown")
            {
                Origin = new Vector2(64, 96)
            };
        }

        public override void OnAddedToScene()
        {
            map = Scene.Services.GetService<GameMap>();
            wallManager = Scene.Services.GetService<WallManager>();
            defenceManager = Scene.Services.GetService<DefenceManager>();
            castle = Scene.Services.GetService<Castle>();

            swordAttackSound = Scene.Content.Load<SoundEffect>(ContentPaths.Sounds.SwordAttack);

            defenceManager.DefenceAdded += DefenceManager_DefenceAdded;
            defenceManager.DefenceRemoved += DefenceManager_DefenceRemoved;

            wallManager.WallAdded += WallManager_WallAdded;
            wallManager.WallRemoved += WallManager_WallRemoved;
        }

        private void DefenceManager_DefenceAdded(Defence defence)
        {
            if (State != EnemyState.Attacking && target != null)
            {
                // Make the new defence the target if it is closer
                // than the current one.
                var oldDistance = (target.Position - Position).LengthSquared();
                var newDistance = (defence.Position - Position).LengthSquared();
                if (newDistance < oldDistance)
                {
                    target = defence;
                    path = null;
                    State = EnemyState.Idle;
                }
            }
        }

        private void DefenceManager_DefenceRemoved(Defence defence)
        {
            // Reset the target if the defence that was
            // removed was the target.
            if (target == defence)
            {
                target = null;
                path = null;
                State = EnemyState.Idle;
                PlayAnimationFor(EnemyState.Idle, Facing);
                stopwatch.Reset();
            }
        }

        private void WallManager_WallAdded(Wall wall)
        {
            // If not currently attacking, reset the path.
            if (State != EnemyState.Attacking)
            {
                path = null;
                State = EnemyState.Idle;
            }
        }

        private void WallManager_WallRemoved(Wall wall)
        {
            // Reset the target if it is currently set to
            // the wall that was just removed.
            if (target == wall)
            {
                target = null;
                path = null;
                State = EnemyState.Idle;
                PlayAnimationFor(EnemyState.Idle, Facing);
                stopwatch.Reset();
            }
            // If not currently attacking, reset the path.
            else if (State != EnemyState.Attacking)
            {
                path = null;
                State = EnemyState.Idle;
            }
        }

        public override void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);

            switch (State)
            {
                case EnemyState.Idle:
                    Update_Idle(gameTime);
                    break;

                case EnemyState.Moving:
                    Update_Moving(gameTime);
                    break;

                case EnemyState.Attacking:
                    Update_Attacking(gameTime);
                    break;
            }

            sprite.Position = map.MapToScreen(Position);
            sprite.Depth = (Position.Y / map.TiledMap.Height) * (Position.X / map.TiledMap.Width);
        }

        public override void Draw(GameTime gameTime)
        {
            Scene.SpriteBatch.Draw(sprite);
            Scene.SpriteBatch.DrawPoint(sprite.Position, Color.Red, 3);

            if (Health < MaxHealth)
            {
                var healthbar = new RectangleF(sprite.Position.X - 40, sprite.Position.Y - 96, 80, 10);
                DrawHealthbar(healthbar, Color.Red);
            }
        }

        private void Update_Idle(GameTime gameTime)
        {
            // Find a target if none is set.
            if (target == null)
            {
                target = defenceManager.GetNearestDefence(Position);

                if ((target == null
                    || (target.Position - Position).Length() < (castle.Position - Position).Length())
                    && !castle.IsDestroyed)
                {
                    target = castle;
                }
            }
            // Find a path to the target if no path has been found.
            else if (path == null)
            {
                var tile = map[(int)Position.X, (int)Position.Y];
                var targetTile = map[(int)target.Position.X, (int)target.Position.Y];
                path = map.GetPath(tile, targetTile, !IsFlying);
            }
            else if (path.Count > 0)
            {
                // If the next tile in the path contains a wall,
                // set that as the target and attack it.
                if (wallManager.CheckContainsWall(path.Last(), out Wall wall))
                {
                    target = wall;
                    path.Clear();
                }
                // Move to the next tile in the path.
                else
                {
                    var displacement = TargetTilePosition - Position;
                    displacement.Normalize();
                    velocity = displacement * 0.03f * MovementSpeedMultiplier;

                    State = EnemyState.Moving;
                    Facing = Utilities.GetDirectionFromVector(velocity);
                    PlayAnimationFor(EnemyState.Moving, Facing);
                }
            }
            // Start attacking the target.
            else
            {
                Vector2 displacement = target.Position - Position + new Vector2(0.5f);
                Facing = Utilities.GetDirectionFromVector(displacement);
                State = EnemyState.Attacking;
                DoAttack();
            }
        }

        private void Update_Moving(GameTime gameTime)
        {
            // Check if the enemy has approximately reached
            // the destination.
            if (Position.EqualsWithTolerence(TargetTilePosition, VectorEqualityTolerance))
            {
                path.RemoveAt(path.Count - 1);
                State = EnemyState.Idle;
            }
            else
            {
                Position += velocity;
            }
        }

        private void Update_Attacking(GameTime gameTime)
        {
            // Reset the target if it has been destroyed.
            if (target.IsDestroyed)
            {
                target = null;
                path = null;
                stopwatch.Reset();

                PlayAnimationFor(EnemyState.Idle, Facing);
                State = EnemyState.Idle;
            }
            // Do an attack if enough time has passed since the last one.
            else if (stopwatch.ElapsedMilliseconds > AttackInterval)
            {
                DoAttack();
                stopwatch.Reset();
            }
        }

        /// <summary>
        /// Deals damage to the current target after playing
        /// the attack animation.
        /// </summary>
        private void DoAttack()
        {
            swordAttackSound.Play();
            PlayAnimationFor(EnemyState.Attacking, Facing, () =>
            {
                if (!target.IsDestroyed)
                {
                    target.Attack(AttackDamage);
                    PlayAnimationFor(EnemyState.Idle, Facing);
                    stopwatch.Start();
                }
            });
        }

        /// <summary>
        /// Plays the animation for the given state and direction.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="direction"></param>
        private void PlayAnimationFor(EnemyState state, Direction direction, Action onCompleted = null)
        {
            sprite.Play(state.ToString() + direction.ToString(), onCompleted);
        }
    }
}
