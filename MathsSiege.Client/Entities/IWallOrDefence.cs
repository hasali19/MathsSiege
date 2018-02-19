using Microsoft.Xna.Framework;

namespace MathsSiege.Client.Entities
{
    public interface IWallOrDefence : IAttackable
    {
        Vector2 Position { get; }
    }
}
