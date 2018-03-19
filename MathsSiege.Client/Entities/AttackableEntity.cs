using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;

namespace MathsSiege.Client.Entities
{
    public class AttackableEntity : DrawableEntity, IAttackable
    {
        private int maxHealth = 100;
        /// <summary>
        /// The maximum health value of the entity.
        /// </summary>
        public int MaxHealth
        {
            get => this.maxHealth;
            set
            {
                this.maxHealth = MathHelper.Max(value, 1);
                this.Health = this.MaxHealth;
            }
        }

        private int health;
        /// <summary>
        /// The current health value of the entity.
        /// </summary>
        public int Health
        {
            get => this.health;
            protected set => this.health = MathHelper.Clamp(value, 0, this.MaxHealth);
        }

        /// <summary>
        /// Whether the entity has been destroyed (health is zero).
        /// </summary>
        public bool IsDestroyed => this.Health == 0;

        /// <summary>
        /// Invoked when the entity is destroyed.
        /// </summary>
        public event Action<AttackableEntity> Destroyed;

        public AttackableEntity()
        {
            this.Health = this.MaxHealth;
        }

        /// <summary>
        /// Attacks the entity.
        /// </summary>
        /// <param name="damage">The amount of damage to deal.</param>
        public void Attack(int damage)
        {
            if (this.IsDestroyed)
            {
                return;
            }

            this.Health -= damage;

            if (this.IsDestroyed)
            {
                this.OnDestroyed();
            }
        }

        protected virtual void OnDestroyed()
        {
            this.Destroyed?.Invoke(this);
        }

        protected virtual void DrawHealthbar(RectangleF rectangle, Color color)
        {
            var normalisedHealth = (float)this.Health / this.MaxHealth;

            this.Scene.SpriteBatch.DrawRectangle(rectangle, color, 1);
            this.Scene.SpriteBatch.FillRectangle(new RectangleF(rectangle.X, rectangle.Y,
                normalisedHealth * rectangle.Width, rectangle.Height), color, 1);
        }
    }
}
