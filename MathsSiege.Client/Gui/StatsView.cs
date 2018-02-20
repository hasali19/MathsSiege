using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using System;

namespace MathsSiege.Client.Gui
{
    public class StatsView : Panel
    {
        /// <summary>
        /// Gets the time that has passed since this view
        /// was first updated.
        /// </summary>
        public TimeSpan ElapsedTime { get; private set; }

        /// <summary>
        /// Gets a formatted string representation of <see cref="ElapsedTime"/>.
        /// </summary>
        private string ElapsedTimeString => this.ElapsedTime.ToString(@"hh\:mm\:ss");

        private DateTime startTime;

        private Paragraph elapsedTimeView;

        public StatsView(Vector2 size, PanelSkin skin = PanelSkin.None, Anchor anchor = Anchor.TopRight)
            : base(size, skin, anchor, null)
        {
            this.Padding = new Vector2(5f);

            this.elapsedTimeView = new Paragraph(this.ElapsedTimeString, Anchor.TopRight);
            this.AddChild(this.elapsedTimeView);
        }

        protected override void DoOnFirstUpdate()
        {
            base.DoOnFirstUpdate();
            this.startTime = DateTime.Now;
        }

        public override void Update(ref Entity targetEntity, ref Entity dragTargetEntity, ref bool wasEventHandled, Point scrollVal)
        {
            base.Update(ref targetEntity, ref dragTargetEntity, ref wasEventHandled, scrollVal);
            
            this.ElapsedTime = DateTime.Now - this.startTime;
            this.elapsedTimeView.Text = this.ElapsedTimeString;
        }
    }
}
