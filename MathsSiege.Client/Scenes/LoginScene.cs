using GeonBit.UI.Entities;
using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MathsSiege.Client.Scenes
{
    public class LoginScene : Scene
    {
        private UserPreferences preferences;
        private DataClient client;

        private TextInput usernameInput;
        private TextInput passwordInput;
        private Paragraph output;

        bool isLoggedIn = false;

        public LoginScene(Game game) : base(game)
        {
            this.preferences = this.Game.Services.GetService<UserPreferences>();
            this.client = this.Game.Services.GetService<DataClient>();
        }

        public override void Initialise()
        {
            base.Initialise();

            var background = this.Content.Load<Texture2D>(ContentPaths.Textures.Background);

            this.UserInterface.Root.Padding = new Vector2(30f);

            var titleContainer = new Panel(new Vector2(this.GraphicsDevice.Viewport.Width - 60, 120), PanelSkin.None, Anchor.TopCenter);
            var title = new Header("Login", Anchor.Center) { FillColor = Color.White };

            var contentContainer = new Panel(new Vector2(this.GraphicsDevice.Viewport.Width - 60,
                this.GraphicsDevice.Viewport.Height - 280), PanelSkin.None, Anchor.AutoCenter);

            this.usernameInput = new TextInput(false)
            {
                PlaceholderText = "Enter your username"
            };

            this.passwordInput = new TextInput(false)
            {
                PlaceholderText = "Enter your password",
                HideInputWithChar = '*'
            };

            this.output = new Paragraph("");

            var back = new Button("Back", anchor: Anchor.BottomLeft, size: new Vector2(200, 70));
            var login = new Button("Login", anchor: Anchor.BottomRight, size: new Vector2(200, 70));

            titleContainer.AddChild(title);

            contentContainer.AddChild(this.usernameInput);
            contentContainer.AddChild(this.passwordInput);
            contentContainer.AddChild(this.output);

            this.UserInterface.AddEntity(titleContainer);
            this.UserInterface.AddEntity(contentContainer);
            this.UserInterface.AddEntity(back);
            this.UserInterface.AddEntity(login);

            back.OnClick = this.Back_OnClick;
            login.OnClick = this.Login_OnClick;

            this.BackgroundImage = background;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.isLoggedIn)
            {
                this.SceneManager.ReplaceScene(new MainScene(this.Game));
            }
        }

        private void Back_OnClick(Entity entity)
        {
            this.SceneManager.PopScene();
        }

        private async void Login_OnClick(Entity entity)
        {
            var loginTask = this.client.LoginAsync(this.usernameInput.Value, this.passwordInput.Value);
            this.output.Text = "Logging in...";
            bool loginSuccess = await loginTask;

            if (!loginSuccess)
            {
                this.output.Text = "Failed to login to the server.\nMake sure you are connected to the internet and check your username and password.";
                return;
            }

            var questionsTask = this.client.LoadQuestionsAsync();
            this.output.Text = "Downloading questions...";
            bool questionsSuccess = await questionsTask;

            if (!questionsSuccess)
            {
                this.output.Text = "Failed to download questions from the server.\nMake sure you are connected to the internet and try again.";
                return;
            }

            this.isLoggedIn = true;
        }
    }
}
