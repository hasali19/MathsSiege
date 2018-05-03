using GeonBit.UI.Entities;
using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;

namespace MathsSiege.Client.Scenes
{
    public class GameOverScene : Scene
    {
        public GameOverScene(Game game) : base(game)
        {
        }

        public override void Initialise()
        {
            base.Initialise();

            ClearColor = Color.Black * 0.5f;

            UserInterface.Root.Padding = new Vector2(0, 100);

            var title = new Header("Game Over");
            var sub = new Paragraph("Your defences have all been destroyed!") { AlignToCenter = true };

            var container = new Panel(new Vector2(300, 190), PanelSkin.None);
            
            var mainMenu = new Button("Main Menu");
            var exit = new Button("Exit Game");
            
            container.AddChild(mainMenu);
            container.AddChild(exit);

            UserInterface.AddEntity(title);
            UserInterface.AddEntity(sub);
            UserInterface.AddEntity(container);
            
            mainMenu.OnClick = (e) => SceneManager.Clear(new MainMenuScene(Game));
            exit.OnClick = (e) => Game.Exit();
        }
    }
}
