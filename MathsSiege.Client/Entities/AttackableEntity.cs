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
            get => maxHealth;
            set
            {
                maxHealth = MathHelper.Max(value, 1);
                Health = MaxHealth;
            }
        }

        private int health;
        /// <summary>
        /// The current health value of the entity.
        /// </summary>
        public int Health
        {
            get => health;
            protected set => health = MathHelper.Clamp(value, 0, MaxHealth);
        }

        /// <summary>
        /// Whether the entity has been destroyed (health is zero).
        /// </summary>
        public bool IsDestroyed => Health == 0;

        /// <summary>
        /// Invoked when the entity is destroyed.
        /// </summary>
        public event Action<AttackableEntity> Destroyed;

        public AttackableEntity()
        {
            Health = MaxHealth;
        }

        /// <summary>
        /// Attacks the entity.
        /// </summary>
        /// <param name="damage">The amount of damage to deal.</param>
        public void Attack(int damage)
        {
            if (IsDestroyed)
            {
                return;
            }

            Health -= damage;

            if (IsDestroyed)
            {
                OnDestroyed();
            }
        }

        protected virtual void OnDestroyed()
        {
            Destroyed?.Invoke(this);
        }

        protected virtual void DrawHealthbar(RectangleF rectangle, Color color)
        {
            var normalisedHealth = (float)Health / MaxHealth;

            Scene.SpriteBatch.DrawRectangle(rectangle, color, 1);
            Scene.SpriteBatch.FillRectangle(new RectangleF(rectangle.X, rectangle.Y,
                normalisedHealth * rectangle.Width, rectangle.Height), color, 1);
        }
    }
}
