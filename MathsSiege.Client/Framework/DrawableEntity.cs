using Microsoft.Xna.Framework;

namespace MathsSiege.Client.Framework
{
    /// <summary>
    /// A drawable game entity.
    /// </summary>
    public abstract class DrawableEntity
    {
        /// <summary>
        /// The scene to which the entity belongs.
        /// </summary>
        public Scene Scene { get; set; }

        /// <summary>
        /// Whether the entity should be updated.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Whether the entity should be drawn.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Called when the entity is added to a scene.
        /// </summary>
        public virtual void OnAddedToScene()
        {
        }

        /// <summary>
        /// Called when the entity is removed from a scene.
        /// </summary>
        public virtual void OnRemovedFromScene()
        {
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime)
        {
        }

        /// <summary>
        /// Draws the entity.
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Draw(GameTime gameTime)
        {
        }
    }
}
