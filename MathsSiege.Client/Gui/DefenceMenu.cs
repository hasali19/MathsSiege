using GeonBit.UI.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MathsSiege.Client.Gui
{
    public class DefenceMenu : Panel
    {
        /// <summary>
        /// The currently selected item in the menu.
        /// </summary>
        public DefenceMenuItem SelectedItem { get; private set; }

        /// <summary>
        /// Invoked when a menu item is clicked.
        /// </summary>
        public event Action ItemClicked;

        private PlayerStats stats;

        public DefenceMenu(PlayerStats stats, Vector2 size, PanelSkin skin = PanelSkin.Simple, Anchor anchor = Anchor.CenterLeft, Vector2? offset = null)
            : base(size, skin, anchor, offset)
        {
            this.stats = stats;

            Padding = new Vector2(10);
            PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll;
        }

        /// <summary>
        /// Adds a new defence to the menu.
        /// </summary>
        /// <param name="name">The name of the defence.</param>
        /// <param name="image">The image of the defence.</param>
        public void AddItem(string name, Texture2D image, int cost)
        {
            var item = new DefenceMenuItem(new Vector2(Size.X - Padding.X * 2 - Scrollbar.Size.X),
                name, image, cost);

            item.Click += (e) =>
            {
                ItemClicked?.Invoke();

                // Set the current selection to the default color if not null.
                if (SelectedItem != null)
                {
                    SelectedItem.FillColor = Color.White;
                }

                // If this is the current selection, clear it.
                if (e == SelectedItem)
                {
                    SelectedItem = null;
                }
                // If the player has enough points, select this item.
                else if (stats.Points >= e.Cost)
                {
                    SelectedItem = e;
                    SelectedItem.FillColor = Color.RoyalBlue;
                }
            };

            AddChild(item);
        }
    }

    /// <summary>
    /// Represents an item in a <see cref="DefenceMenuItem"/>.
    /// </summary>
    public class DefenceMenuItem : Panel
    {
        /// <summary>
        /// The name of the defence.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The points cost of the defence.
        /// </summary>
        public int Cost { get; set; }

        /// <summary>
        /// Invoked when the item is clicked.
        /// </summary>
        public event Action<DefenceMenuItem> Click;

        public DefenceMenuItem(Vector2 size, string name, Texture2D image, int cost, PanelSkin skin = PanelSkin.Default, Anchor anchor = Anchor.Auto)
            : base(size, skin, anchor, null)
        {
            Name = name;
            Cost = cost;
            Padding = new Vector2(10);

            AddChild(new Image(image, Size - Padding * 2, ImageDrawMode.Stretch, Anchor.TopCenter)
            {
                ClickThrough = true
            });

            AddChild(new Paragraph(name, Anchor.TopLeft)
            {
                ClickThrough = true
            });

            AddChild(new Label(cost.ToString() + " Points", Anchor.BottomRight)
            {
                ClickThrough = true
            });

            OnClick = (e) => Click?.Invoke(this);
        }
    }
}
