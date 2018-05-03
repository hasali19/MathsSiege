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
            preferences = Game.Services.GetService<UserPreferences>();
            client = Game.Services.GetService<DataClient>();
        }

        public override void Initialise()
        {
            base.Initialise();

            var background = Content.Load<Texture2D>(ContentPaths.Textures.Background);

            UserInterface.Root.Padding = new Vector2(30f);

            var titleContainer = new Panel(new Vector2(GraphicsDevice.Viewport.Width - 60, 120), PanelSkin.None, Anchor.TopCenter);
            var title = new Header("Login", Anchor.Center) { FillColor = Color.White };

            var contentContainer = new Panel(new Vector2(GraphicsDevice.Viewport.Width - 60,
                GraphicsDevice.Viewport.Height - 280), PanelSkin.None, Anchor.AutoCenter);

            usernameInput = new TextInput(false)
            {
                PlaceholderText = "Enter your username"
            };

            passwordInput = new TextInput(false)
            {
                PlaceholderText = "Enter your password",
                HideInputWithChar = '*'
            };

            output = new Paragraph("");

            var back = new Button("Back", anchor: Anchor.BottomLeft, size: new Vector2(200, 70));
            var login = new Button("Login", anchor: Anchor.BottomRight, size: new Vector2(200, 70));

            titleContainer.AddChild(title);

            contentContainer.AddChild(usernameInput);
            contentContainer.AddChild(passwordInput);
            contentContainer.AddChild(output);

            UserInterface.AddEntity(titleContainer);
            UserInterface.AddEntity(contentContainer);
            UserInterface.AddEntity(back);
            UserInterface.AddEntity(login);

            back.OnClick = Back_OnClick;
            login.OnClick = Login_OnClick;

            BackgroundImage = background;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (isLoggedIn)
            {
                SceneManager.ReplaceScene(new MainScene(Game));
            }
        }

        private void Back_OnClick(Entity entity)
        {
            SceneManager.PopScene();
        }

        private async void Login_OnClick(Entity entity)
        {
            var loginTask = client.LoginAsync(usernameInput.Value, passwordInput.Value);
            output.Text = "Logging in...";
            bool loginSuccess = await loginTask;

            if (!loginSuccess)
            {
                output.Text = "Failed to login to the server.\nMake sure you are connected to the internet and check your username and password.";
                return;
            }

            var questionsTask = client.LoadQuestionsAsync();
            output.Text = "Downloading questions...";
            bool questionsSuccess = await questionsTask;

            if (!questionsSuccess)
            {
                output.Text = "Failed to download questions from the server.\nMake sure you are connected to the internet and try again.";
                return;
            }

            isLoggedIn = true;
        }
    }
}
