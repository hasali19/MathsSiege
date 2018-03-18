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

            this.ClearColor = Color.Black * 0.5f;

            this.UserInterface.Root.Padding = new Vector2(0, 100);

            var title = new Header("Game Over");
            var sub = new Paragraph("Your defences have all been destroyed!") { AlignToCenter = true };

            var container = new Panel(new Vector2(300, 190), PanelSkin.None);
            
            var mainMenu = new Button("Main Menu");
            var exit = new Button("Exit Game");
            
            container.AddChild(mainMenu);
            container.AddChild(exit);

            this.UserInterface.AddEntity(title);
            this.UserInterface.AddEntity(sub);
            this.UserInterface.AddEntity(container);
            
            mainMenu.OnClick = (e) => this.SceneManager.Clear(new MainMenuScene(this.Game));
            exit.OnClick = (e) => this.Game.Exit();
        }
    }
}
