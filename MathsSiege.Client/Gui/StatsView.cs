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

        public Button AddPointsButton;

        /// <summary>
        /// Gets a formatted string representation of <see cref="ElapsedTime"/>.
        /// </summary>
        private string ElapsedTimeString => this.ElapsedTime.ToString(@"hh\:mm\:ss");

        /// <summary>
        /// Gets the string to display the player's current points.
        /// </summary>
        private string PointsString => $"{this.stats.Points} Points";

        private PlayerStats stats;

        private DateTime startTime;

        private Paragraph elapsedTimeView;
        private Paragraph pointsView;

        public StatsView(PlayerStats stats, Vector2 size, PanelSkin skin = PanelSkin.None, Anchor anchor = Anchor.TopRight)
            : base(size, skin, anchor, null)
        {
            this.stats = stats;
            this.Padding = new Vector2(5f);

            this.elapsedTimeView = new Paragraph(this.ElapsedTimeString, Anchor.Auto);
            this.pointsView = new Paragraph(this.PointsString, Anchor.Auto);

            this.AddPointsButton = new Button("Add Points", size: new Vector2(size.X - this.Padding.X * 2, 60))
            {
                Padding = new Vector2(5f)
            };

            this.AddPointsButton.ButtonParagraph.Scale = 0.8f;

            this.AddChild(this.elapsedTimeView);
            this.AddChild(this.pointsView);
            this.AddChild(this.AddPointsButton);
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

            this.pointsView.Text = this.PointsString;
        }
    }
}
