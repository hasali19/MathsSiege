using GeonBit.UI.Entities;
using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MathsSiege.Client.Scenes
{
    public class SettingsScene : Scene
    {
        private UserPreferences preferences;

        private TextInput addressInput;

        public SettingsScene(Game game) : base(game)
        {
            this.UserInterface.UseRenderTarget = true;
            this.preferences = this.Game.Services.GetService<UserPreferences>();
        }

        public override void Initialise()
        {
            base.Initialise();

            var background = this.Content.Load<Texture2D>(ContentPaths.Textures.SplashBackground);

            this.UserInterface.Root.Padding = new Vector2(30f);

            var titleContainer = new Panel(new Vector2(this.GraphicsDevice.Viewport.Width - 60, 120), PanelSkin.None, Anchor.TopCenter);
            var title = new Header("Settings", Anchor.Center) { FillColor = Color.White };

            var contentContainer = new Panel(new Vector2(this.GraphicsDevice.Viewport.Width - 60,
                this.GraphicsDevice.Viewport.Height - 280), PanelSkin.None, Anchor.AutoCenter)
            {
                PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll
            };

            this.addressInput = new TextInput(false)
            {
                PlaceholderText = "Enter the server address",
                Value = string.IsNullOrWhiteSpace(this.preferences.HostAddress) ? "" : this.preferences.HostAddress
            };

            var back = new Button("Back", anchor: Anchor.BottomLeft, size: new Vector2(200, 70));
            var save = new Button("Save", anchor: Anchor.BottomRight, size: new Vector2(200, 70));

            titleContainer.AddChild(title);

            contentContainer.AddChild(this.addressInput);

            this.UserInterface.AddEntity(titleContainer);
            this.UserInterface.AddEntity(contentContainer);
            this.UserInterface.AddEntity(back);
            this.UserInterface.AddEntity(save);

            back.OnClick = this.Back_OnClick;
            save.OnClick = this.Save_OnClick;

            this.BackgroundImage = background;
        }

        private void Back_OnClick(Entity entity)
        {
            this.SceneManager.PopScene();
        }

        private void Save_OnClick(Entity entity)
        {
            this.preferences.HostAddress = this.addressInput.Value;
            this.preferences.Save();
            this.SceneManager.PopScene();
        }
    }
}
