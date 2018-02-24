using GeonBit.UI.Entities;
using MathsSiege.Client.Framework;
using MathsSiege.Models;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Diagnostics;
using System.Linq;

namespace MathsSiege.Client.Scenes
{
    public class QuestionScene : Scene
    {
        private const int PauseAfterAnswer = 1000;

        private PlayerStats stats;
        private DataClient client;
        private Question question;

        private Stopwatch stopwatch = new Stopwatch();

        private Panel choicesContainer;
        private Label points;

        private bool isAnswered;

        public QuestionScene(Game game) : base(game)
        {
            this.client = this.Game.Services.GetService<DataClient>();
            this.stats = this.Game.Services.GetService<PlayerStats>();

            this.question = this.client.GetRandomQuestion();

            this.UserInterface.UseRenderTarget = true;
        }

        public override void Initialise()
        {
            base.Initialise();

            this.ClearColor = Color.Transparent;

            this.UserInterface.Root.Padding = new Vector2(30f);

            var question = new Header(this.question.Text);
            this.points = new Label(this.question.GetPoints().ToString() + " Points") { AlignToCenter = true };

            this.choicesContainer = new Panel(new Vector2(500, 0), PanelSkin.None, Anchor.AutoCenter);
            
            foreach (var choice in this.question.GetRandomizedChoices())
            {
                var button = new Button(choice.Text, ButtonSkin.Alternative);
                this.choicesContainer.AddChild(button);
                button.OnClick = (e) => this.ChoiceButton_OnClick(button, choice);
            }

            this.UserInterface.AddEntity(question);
            this.UserInterface.AddEntity(this.points);
            this.UserInterface.AddEntity(this.choicesContainer);
        }

        private void ChoiceButton_OnClick(Button button, Choice choice)
        {
            if (choice.IsCorrect)
            {
                this.stopwatch.Start();

                // Add the answer if it was the first one picked.
                if (!this.isAnswered)
                {
                    this.points.FillColor = Color.Green;
                    this.stats.AddAnswer(choice);
                }

                // Disable all other buttons.
                foreach (var b in this.choicesContainer.GetChildren()
                    .Where(entity => entity is Button btn && btn != button))
                {
                    b.Disabled = true;
                }
            }
            else
            {
                button.Disabled = true;

                // Add the answer if it was the first one picked.
                if (!this.isAnswered)
                {
                    this.points.FillColor = Color.Red;
                    this.stats.AddAnswer(choice);
                }
            }

            this.isAnswered = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Go back to the previous scene after a short delay.
            if (this.stopwatch.ElapsedMilliseconds > PauseAfterAnswer)
            {
                this.SceneManager.PopScene();
            }
        }

        protected override void DrawBackground()
        {
            this.SpriteBatch.FillRectangle(this.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
        }
    }
}
