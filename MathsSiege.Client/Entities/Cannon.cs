using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended.TextureAtlases;

namespace MathsSiege.Client.Entities
{
    public class Cannon : Defence
    {
        private ProjectileManager projectileManager;

        private SoundEffect cannonFireSound;

        public Cannon(TextureAtlas atlas) : base(atlas)
        {
        }

        public override void OnAddedToScene()
        {
            base.OnAddedToScene();

            projectileManager = Scene.Services.GetService<ProjectileManager>();
            cannonFireSound = Scene.Content.Load<SoundEffect>(ContentPaths.Sounds.CannonFire);
        }

        protected override void DoAttack(Enemy target)
        {
            // Fire a cannonball at the target.
            projectileManager
                .CreateProjectile(ProjectileType.Cannonball)
                .Fire(Position, target.Position, 60);

            cannonFireSound.Play();
        }
    }
}
