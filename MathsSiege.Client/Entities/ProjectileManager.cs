using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;

namespace MathsSiege.Client.Entities
{
    public enum ProjectileType
    {
        Cannonball
    }

    public class ProjectileManager : DrawableEntity
    {
        private Bag<Projectile> projectiles = new Bag<Projectile>();

        private Texture2D cannonball;

        /// <summary>
        /// Creates a new projectile of the specified type.
        /// </summary>
        /// <param name="type">The type of projectile.</param>
        /// <returns></returns>
        public Projectile CreateProjectile(ProjectileType type)
        {
            Projectile projectile = null;

            switch (type)
            {
                case ProjectileType.Cannonball:
                    projectile = new Projectile(cannonball) { Acceleration = new Vector3(0, 0, -30.0f) };
                    break;

                default:
                    return null;
            }

            projectile.Scene = Scene;

            projectiles.Add(projectile);
            projectile.OnAddedToScene();

            projectile.TargetReached += (p) => projectiles.Remove(projectile);

            return projectile;
        }

        public override void OnAddedToScene()
        {
            cannonball = Scene.Content.Load<Texture2D>(ContentPaths.Textures.Cannonball);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var projectile in projectiles)
            {
                projectile.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var project in projectiles)
            {
                project.Draw(gameTime);
            }
        }
    }
}
