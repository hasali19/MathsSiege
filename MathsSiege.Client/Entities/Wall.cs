﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;

namespace MathsSiege.Client.Entities
{
    public class Wall : AttackableEntity, IEnemyTarget
    {
        public Vector2 Position { get; set; }
        
        private Sprite sprite;

        public Wall(Texture2D texture)
        {
            this.sprite = new Sprite(texture)
            {
                Origin = new Vector2(32, 32)
            };
        }

        public override void OnAddedToScene()
        {
            var map = this.Scene.Services.GetService<GameMap>();
            this.sprite.Position = map.MapToScreen(this.Position);
            this.sprite.Depth = (this.Position.Y / map.TiledMap.Height) * (this.Position.X / map.TiledMap.Width);
        }

        public override void Draw(GameTime gameTime)
        {
            this.Scene.SpriteBatch.Draw(this.sprite);

            if (this.Health < this.MaxHealth)
            {
                var healthbar = new RectangleF(this.sprite.Position.X - 40, this.sprite.Position.Y - 32, 80, 10);
                this.DrawHealthbar(healthbar, Color.Red);
            }
        }
    }
}
