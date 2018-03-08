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
            get => this.sprite.Origin;
            set => this.sprite.Origin = value;
        }

        public Vector2 Scale
        {
            get => this.sprite.Scale;
            set => this.sprite.Scale = value;
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

        private AnimatedSprite sprite;

        private SoundEffect swordAttackSound;

        private IWallOrDefence target;
        private IList<Tile> path;

        private Vector2 velocity;

        private Stopwatch stopwatch = new Stopwatch();

        private Vector2 TargetTilePosition => this.path.Last().Position + new Vector2(0.5f);

        public Enemy(SpriteSheetAnimationFactory animationFactory)
        {
            this.sprite = new AnimatedSprite(animationFactory, "IdleDown")
            {
                Origin = new Vector2(64, 96)
            };
        }

        public override void OnAddedToScene()
        {
            this.map = this.Scene.Services.GetService<GameMap>();
            this.wallManager = this.Scene.Services.GetService<WallManager>();
            this.defenceManager = this.Scene.Services.GetService<DefenceManager>();

            this.swordAttackSound = this.Scene.Content.Load<SoundEffect>(ContentPaths.Sounds.SwordAttack);

            this.defenceManager.DefenceAdded += this.DefenceManager_DefenceAdded;
            this.defenceManager.DefenceRemoved += this.DefenceManager_DefenceRemoved;

            this.wallManager.WallAdded += this.WallManager_WallAdded;
            this.wallManager.WallRemoved += this.WallManager_WallRemoved;
        }

        private void DefenceManager_DefenceAdded(Defence defence)
        {
            if (this.State != EnemyState.Attacking && this.target != null)
            {
                // Make the new defence the target if it is closer
                // than the current one.
                var oldDistance = (this.target.Position - this.Position).LengthSquared();
                var newDistance = (defence.Position - this.Position).LengthSquared();
                if (newDistance < oldDistance)
                {
                    this.target = defence;
                    this.path = null;
                    this.State = EnemyState.Idle;
                }
            }
        }

        private void DefenceManager_DefenceRemoved(Defence defence)
        {
            // Reset the target if the defence that was
            // removed was the target.
            if (this.target == defence)
            {
                this.target = null;
                this.path = null;
                this.State = EnemyState.Idle;
                this.PlayAnimationFor(EnemyState.Idle, this.Facing);
                this.stopwatch.Reset();
            }
        }

        private void WallManager_WallAdded(Wall wall)
        {
            // If not currently attacking, reset the path.
            if (this.State != EnemyState.Attacking)
            {
                this.path = null;
                this.State = EnemyState.Idle;
            }
        }

        private void WallManager_WallRemoved(Wall wall)
        {
            // Reset the target if it is currently set to
            // the wall that was just removed.
            if (this.target == wall)
            {
                this.target = null;
                this.path = null;
                this.State = EnemyState.Idle;
                this.PlayAnimationFor(EnemyState.Idle, this.Facing);
                this.stopwatch.Reset();
            }
            // If not currently attacking, reset the path.
            else if (this.State != EnemyState.Attacking)
            {
                this.path = null;
                this.State = EnemyState.Idle;
            }
        }

        public override void Update(GameTime gameTime)
        {
            this.sprite.Update(gameTime);

            switch (this.State)
            {
                case EnemyState.Idle:
                    this.Update_Idle(gameTime);
                    break;

                case EnemyState.Moving:
                    this.Update_Moving(gameTime);
                    break;

                case EnemyState.Attacking:
                    this.Update_Attacking(gameTime);
                    break;
            }

            this.sprite.Position = this.map.MapToScreen(this.Position);
            this.sprite.Depth = (this.Position.Y / this.map.TiledMap.Height) * (this.Position.X / this.map.TiledMap.Width);
        }

        public override void Draw(GameTime gameTime)
        {
            this.Scene.SpriteBatch.Draw(this.sprite);
            this.Scene.SpriteBatch.DrawPoint(this.sprite.Position, Color.Red, 3);

            if (this.Health < this.MaxHealth)
            {
                var healthbar = new RectangleF(this.sprite.Position.X - 40, this.sprite.Position.Y - 96, 80, 10);
                this.DrawHealthbar(healthbar, Color.Red);
            }
        }

        private void Update_Idle(GameTime gameTime)
        {
            // Find a target if none is set.
            if (this.target == null)
            {
                this.target = this.defenceManager.GetNearestDefence(this.Position);
            }
            // Find a path to the target if no path has been found.
            else if (this.path == null)
            {
                var tile = this.map[(int)this.Position.X, (int)this.Position.Y];
                var targetTile = this.map[(int)this.target.Position.X, (int)this.target.Position.Y];
                this.path = this.map.GetPath(tile, targetTile, !this.IsFlying);
            }
            else if (this.path.Count > 0)
            {
                // If the next tile in the path contains a wall,
                // set that as the target and attack it.
                if (this.wallManager.CheckContainsWall(this.path.Last(), out Wall wall))
                {
                    this.target = wall;
                    this.path.Clear();
                }
                // Move to the next tile in the path.
                else
                {
                    var displacement = this.TargetTilePosition - this.Position;
                    displacement.Normalize();
                    this.velocity = displacement * 0.03f * this.MovementSpeedMultiplier;

                    this.State = EnemyState.Moving;
                    this.Facing = Utilities.GetDirectionFromVector(this.velocity);
                    this.PlayAnimationFor(EnemyState.Moving, this.Facing);
                }
            }
            // Start attacking the target.
            else
            {
                Vector2 displacement = this.target.Position - this.Position + new Vector2(0.5f);
                this.Facing = Utilities.GetDirectionFromVector(displacement);
                this.State = EnemyState.Attacking;
                this.DoAttack();
            }
        }

        private void Update_Moving(GameTime gameTime)
        {
            // Check if the enemy has approximately reached
            // the destination.
            if (this.Position.EqualsWithTolerence(this.TargetTilePosition, VectorEqualityTolerance))
            {
                this.path.RemoveAt(this.path.Count - 1);
                this.State = EnemyState.Idle;
            }
            else
            {
                this.Position += this.velocity;
            }
        }

        private void Update_Attacking(GameTime gameTime)
        {
            // Reset the target if it has been destroyed.
            if (this.target.IsDestroyed)
            {
                this.target = null;
                this.path = null;
                this.stopwatch.Reset();

                this.PlayAnimationFor(EnemyState.Idle, this.Facing);
                this.State = EnemyState.Idle;
            }
            // Do an attack if enough time has passed since the last one.
            else if (this.stopwatch.ElapsedMilliseconds > this.AttackInterval)
            {
                this.DoAttack();
                this.stopwatch.Reset();
            }
        }

        /// <summary>
        /// Deals damage to the current target after playing
        /// the attack animation.
        /// </summary>
        private void DoAttack()
        {
            this.swordAttackSound.Play();
            this.PlayAnimationFor(EnemyState.Attacking, this.Facing, () =>
            {
                if (!this.target.IsDestroyed)
                {
                    this.target.Attack(this.AttackDamage);
                    this.PlayAnimationFor(EnemyState.Idle, this.Facing);
                    this.stopwatch.Start();
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
            this.sprite.Play(state.ToString() + direction.ToString(), onCompleted);
        }
    }
}
