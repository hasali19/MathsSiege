using System;

namespace MathsSiege.Client.Entities
{
    public interface IAttackable
    {
        int Health { get; }
        bool IsDestroyed { get; }
        int MaxHealth { get; }

        event Action<AttackableEntity> Destroyed;

        void Attack(int damage);
    }
}
