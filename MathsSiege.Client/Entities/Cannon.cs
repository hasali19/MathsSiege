using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended.TextureAtlases;

namespace MathsSiege.Client.Entities
{
    public class Cannon : Defence
    {
        private UserPreferences preferences;
        private ProjectileManager projectileManager;

        private SoundEffect cannonFireSound;

        public Cannon(TextureAtlas atlas) : base(atlas)
        {
        }

        public override void OnAddedToScene()
        {
            base.OnAddedToScene();

            preferences = Scene.Services.GetService<UserPreferences>();
            projectileManager = Scene.Services.GetService<ProjectileManager>();
            cannonFireSound = Scene.Content.Load<SoundEffect>(ContentPaths.Sounds.CannonFire);
        }

        protected override void DoAttack(Enemy target)
        {
            // Fire a cannonball at the target.
            projectileManager
                .CreateProjectile(ProjectileType.Cannonball)
                .Fire(Position, target.Position, 60);

            if (preferences.IsAudioEnabled)
            {
                cannonFireSound.Play();
            }
        }
    }
}
