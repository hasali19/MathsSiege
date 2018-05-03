using GeonBit.UI.Entities;
using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;

namespace MathsSiege.Client.Scenes
{
    public class PauseMenuScene : Scene
    {
        public PauseMenuScene(Game game) : base(game)
        {
        }

        public override void Initialise()
        {
            base.Initialise();

            ClearColor = Color.Black * 0.5f;

            var container = new Panel(new Vector2(300, 285), PanelSkin.None);

            var resume = new Button("Resume");
            var mainMenu = new Button("Main Menu");
            var exit = new Button("Exit Game");

            container.AddChild(resume);
            container.AddChild(mainMenu);
            container.AddChild(exit);

            UserInterface.AddEntity(container);

            resume.OnClick = (e) => SceneManager.PopScene();
            mainMenu.OnClick = (e) => SceneManager.Clear(new MainMenuScene(Game));
            exit.OnClick = (e) => Game.Exit();
        }
    }
}
