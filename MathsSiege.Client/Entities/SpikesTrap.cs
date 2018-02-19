using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.TextureAtlases;

namespace MathsSiege.Client.Entities
{
    public class SpikesTrap : Trap
    {
        private Stopwatch stopwatch = new Stopwatch();

        public SpikesTrap(TextureAtlas atlas) : base(atlas)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Destoy the trap after some time has passed
            // since being triggered.
            if (this.stopwatch.IsRunning && this.stopwatch.ElapsedMilliseconds > 2000)
            {
                this.OnDestroyed();
            }
        }

        protected override void OnTrigger(IReadOnlyCollection<Enemy> enemies)
        {
            this.SetTextureRegion(TrapState.Triggered);

            foreach (var enemy in enemies)
            {
                enemy.Attack(20);
            }

            this.stopwatch.Start();
        }
    }
}
