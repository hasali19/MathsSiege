using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.Sprites;

namespace MathsSiege.Client.Entities
{
    public enum EnemyState
    {
        Idle,
        Moving,
        Attacking
    }

    public class Enemy : DrawableEntity
    {
        public Vector2 Position { get; set; }

        public EnemyState State { get; private set; }

        private GameMap map;

        private AnimatedSprite sprite;

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
        }

        private void Update_Idle(GameTime gameTime)
        {
        }

        private void Update_Moving(GameTime gameTime)
        {
        }

        private void Update_Attacking(GameTime gameTime)
        {
        }
    }
}
