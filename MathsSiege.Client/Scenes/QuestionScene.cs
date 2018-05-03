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
            client = Game.Services.GetService<DataClient>();
            stats = Game.Services.GetService<PlayerStats>();

            question = client.GetRandomQuestion();

            UserInterface.UseRenderTarget = true;
        }

        public override void Initialise()
        {
            base.Initialise();

            ClearColor = Color.Transparent;

            UserInterface.Root.Padding = new Vector2(30f);

            var question = new Header(this.question.Text) { Scale = 1.5f };
            points = new Label(this.question.GetPoints().ToString() + " Points") { AlignToCenter = true };

            choicesContainer = new Panel(new Vector2(500, 0), PanelSkin.None, Anchor.AutoCenter);
            
            foreach (var choice in this.question.GetRandomizedChoices())
            {
                var button = new Button(choice.Text, ButtonSkin.Alternative);
                choicesContainer.AddChild(button);
                button.OnClick = (e) => ChoiceButton_OnClick(button, choice);
            }

            UserInterface.AddEntity(question);
            UserInterface.AddEntity(points);
            UserInterface.AddEntity(choicesContainer);
        }

        private void ChoiceButton_OnClick(Button button, Choice choice)
        {
            if (choice.IsCorrect)
            {
                stopwatch.Start();

                // Add the answer if it was the first one picked.
                if (!isAnswered)
                {
                    points.FillColor = Color.Green;
                    stats.AddAnswer(choice);
                }

                // Disable all other buttons.
                foreach (var b in choicesContainer.GetChildren()
                    .Where(entity => entity is Button btn && btn != button))
                {
                    b.Disabled = true;
                }
            }
            else
            {
                button.Disabled = true;

                // Add the answer if it was the first one picked.
                if (!isAnswered)
                {
                    points.FillColor = Color.Red;
                    stats.AddAnswer(choice);
                }
            }

            isAnswered = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Go back to the previous scene after a short delay.
            if (stopwatch.ElapsedMilliseconds > PauseAfterAnswer)
            {
                SceneManager.PopScene();
            }
        }

        protected override void DrawBackground()
        {
            SpriteBatch.FillRectangle(GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);
        }
    }
}
