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

        public DefenceMenu(Vector2 size, PanelSkin skin = PanelSkin.Simple, Anchor anchor = Anchor.CenterLeft, Vector2? offset = null)
            : base(size, skin, anchor, offset)
        {
            this.Padding = new Vector2(10);
            this.PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll;
        }

        /// <summary>
        /// Adds a new defence to the menu.
        /// </summary>
        /// <param name="name">The name of the defence.</param>
        /// <param name="image">The image of the defence.</param>
        public void AddItem(string name, Texture2D image)
        {
            var item = new DefenceMenuItem(new Vector2(this.Size.X - this.Padding.X * 2 - this.Scrollbar.Size.X), name, image);

            item.Click += (e) =>
            {
                this.ItemClicked?.Invoke();

                if (this.SelectedItem != null)
                {
                    this.SelectedItem.FillColor = Color.White;
                }

                if (e == this.SelectedItem)
                {
                    this.SelectedItem = null;
                }
                else
                {
                    this.SelectedItem = e;
                    this.SelectedItem.FillColor = Color.RoyalBlue;
                }
            };

            this.AddChild(item);
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
        /// Invoked when the item is clicked.
        /// </summary>
        public event Action<DefenceMenuItem> Click;

        public DefenceMenuItem(Vector2 size, string name, Texture2D image, PanelSkin skin = PanelSkin.Default, Anchor anchor = Anchor.Auto)
            : base(size, skin, anchor, null)
        {
            this.Name = name;
            this.Padding = new Vector2(10);

            this.AddChild(new Image(image, this.Size - this.Padding * 2, ImageDrawMode.Stretch, Anchor.TopCenter)
            {
                ClickThrough = true
            });

            this.AddChild(new Paragraph(name, Anchor.BottomCenter)
            {
                ClickThrough = true
            });

            this.OnClick = (e) => this.Click?.Invoke(this);
        }
    }
}
