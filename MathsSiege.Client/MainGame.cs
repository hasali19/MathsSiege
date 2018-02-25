﻿using GeonBit.UI;
using MathsSiege.Client.Framework;
using MathsSiege.Client.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MathsSiege.Client
{
    public class MainGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SceneManager sceneManager;
        
        public MainGame()
        {
            this.graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
            };

            this.Content.RootDirectory = "Content";
            
            this.sceneManager = new SceneManager(this);
            this.Components.Add(this.sceneManager);
            this.Services.AddService(this.sceneManager);
        }

        protected override void Initialize()
        {
            UserInterface.Initialize(this.Content, "custom");

            var preferences = new UserPreferences();
            preferences.Load();

            var dataClient = new DataClient(preferences);

            this.Services.AddService(preferences);
            this.Services.AddService(dataClient);

            base.Initialize();
            
            this.sceneManager.PushScene(new MainMenuScene(this));
        }

        protected override void Update(GameTime gameTime)
        {
            InputHandler.Update();

            base.Update(gameTime);
        }
    }
}
