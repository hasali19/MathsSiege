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
            UserInterface.UseRenderTarget = true;
            preferences = Game.Services.GetService<UserPreferences>();
        }

        public override void Initialise()
        {
            base.Initialise();

            var background = Content.Load<Texture2D>(ContentPaths.Textures.SplashBackground);

            UserInterface.Root.Padding = new Vector2(30f);

            var titleContainer = new Panel(new Vector2(GraphicsDevice.Viewport.Width - 60, 120), PanelSkin.None, Anchor.TopCenter);
            var title = new Header("Settings", Anchor.Center) { FillColor = Color.White };

            var contentContainer = new Panel(new Vector2(GraphicsDevice.Viewport.Width - 60,
                GraphicsDevice.Viewport.Height - 280), PanelSkin.None, Anchor.AutoCenter)
            {
                PanelOverflowBehavior = PanelOverflowBehavior.VerticalScroll
            };

            var fullscreenToggle = new CheckBox("Enable Fullscreen Mode (Requires restart)")
            {
                Checked = preferences.IsWindowFullScreen
            };

            addressInput = new TextInput(false)
            {
                PlaceholderText = "Enter the server address",
                Value = string.IsNullOrWhiteSpace(preferences.HostAddress) ? "" : preferences.HostAddress
            };

            var back = new Button("Back", anchor: Anchor.BottomLeft, size: new Vector2(200, 70));
            var save = new Button("Save", anchor: Anchor.BottomRight, size: new Vector2(200, 70));

            titleContainer.AddChild(title);

            contentContainer.AddChild(fullscreenToggle);
            contentContainer.AddChild(addressInput);

            UserInterface.AddEntity(titleContainer);
            UserInterface.AddEntity(contentContainer);
            UserInterface.AddEntity(back);
            UserInterface.AddEntity(save);

            fullscreenToggle.OnValueChange = (e) => preferences.IsWindowFullScreen = fullscreenToggle.Checked;

            back.OnClick = Back_OnClick;
            save.OnClick = Save_OnClick;

            BackgroundImage = background;
        }

        private void Back_OnClick(Entity entity)
        {
            SceneManager.PopScene();
        }

        private void Save_OnClick(Entity entity)
        {
            preferences.HostAddress = addressInput.Value;
            preferences.Save();
            SceneManager.PopScene();
        }
    }
}
