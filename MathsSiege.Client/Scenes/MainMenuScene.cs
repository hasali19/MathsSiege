﻿using GeonBit.UI.Entities;
using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MathsSiege.Client.Scenes
{
    public class MainMenuScene : Scene
    {
        public MainMenuScene(Game game) : base(game)
        {
        }

        public override void Initialise()
        {
            base.Initialise();

            var background = Content.Load<Texture2D>(ContentPaths.Textures.SplashBackground);

            var container = new Panel(new Vector2(300, 285), PanelSkin.None);

            var play = new Button("Play");
            var settings = new Button("Settings");
            var quit = new Button("Quit");

            container.AddChild(play);
            container.AddChild(settings);
            container.AddChild(quit);

            UserInterface.AddEntity(container);

            play.OnClick = Play_OnClick;
            settings.OnClick = Settings_OnClick;
            quit.OnClick = Quit_OnClick;

            BackgroundImage = background;
        }

        private void Play_OnClick(Entity entity)
        {
            SceneManager.PushScene(new LoginScene(Game));
        }

        private void Settings_OnClick(Entity entity)
        {
            SceneManager.PushScene(new SettingsScene(Game));
        }

        private void Quit_OnClick(Entity entity)
        {
            Game.Exit();
        }
    }
}
