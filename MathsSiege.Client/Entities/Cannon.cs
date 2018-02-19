﻿using MonoGame.Extended.TextureAtlases;

namespace MathsSiege.Client.Entities
{
    public class Cannon : Defence
    {
        private ProjectileManager projectileManager;

        public Cannon(TextureAtlas atlas) : base(atlas)
        {
        }

        public override void OnAddedToScene()
        {
            base.OnAddedToScene();

            this.projectileManager = this.Scene.Services.GetService<ProjectileManager>();
        }

        protected override void DoAttack(Enemy target)
        {
            // Fire a cannonball at the target.
            this.projectileManager
                .CreateProjectile(ProjectileType.Cannonball)
                .Fire(this.Position, target.Position, 60);
        }
    }
}
