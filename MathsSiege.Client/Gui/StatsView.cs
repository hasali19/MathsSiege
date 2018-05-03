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
        private string ElapsedTimeString => ElapsedTime.ToString(@"hh\:mm\:ss");

        /// <summary>
        /// Gets the string to display the player's current points.
        /// </summary>
        private string PointsString => $"{stats.Points} Points";

        private PlayerStats stats;

        private DateTime startTime;

        private Paragraph elapsedTimeView;
        private Paragraph pointsView;

        public StatsView(PlayerStats stats, Vector2 size, PanelSkin skin = PanelSkin.None, Anchor anchor = Anchor.TopRight)
            : base(size, skin, anchor, null)
        {
            this.stats = stats;
            Padding = new Vector2(5f);

            elapsedTimeView = new Paragraph(ElapsedTimeString, Anchor.Auto);
            pointsView = new Paragraph(PointsString, Anchor.Auto);

            AddPointsButton = new Button("Add Points", size: new Vector2(size.X - Padding.X * 2, 60))
            {
                Padding = new Vector2(5f)
            };

            AddPointsButton.ButtonParagraph.Scale = 0.8f;

            AddChild(elapsedTimeView);
            AddChild(pointsView);
            AddChild(AddPointsButton);
        }

        protected override void DoOnFirstUpdate()
        {
            base.DoOnFirstUpdate();
            startTime = DateTime.Now;
        }

        public override void Update(ref Entity targetEntity, ref Entity dragTargetEntity, ref bool wasEventHandled, Point scrollVal)
        {
            base.Update(ref targetEntity, ref dragTargetEntity, ref wasEventHandled, scrollVal);

            ElapsedTime = DateTime.Now - startTime;
            elapsedTimeView.Text = ElapsedTimeString;

            pointsView.Text = PointsString;
        }
    }
}
