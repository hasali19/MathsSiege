using Microsoft.Xna.Framework;

namespace MathsSiege.Client.Entities
{
    public interface IEnemyTarget : IAttackable
    {
        Vector2 Position { get; }
    }
}
