﻿using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;

namespace MathsSiege.Client.Entities
{
    public class GameMap : DrawableEntity
    {
        private TiledMapRenderer renderer;

        private Tile[,] tiles;

        public TiledMap TiledMap { get; }

        public GameMap(TiledMap tiledMap)
        {
            this.TiledMap = tiledMap;

            this.tiles = new Tile[tiledMap.Width, tiledMap.Height];

            foreach (var layer in tiledMap.TileLayers)
            {
                for (int x = 0; x < layer.Width; x++)
                {
                    for (int y = 0; y < layer.Height; y++)
                    {
                        if (this.tiles[x, y] == null)
                        {
                            this.tiles[x, y] = new Tile(this, x, y);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the tile at the specified position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Tile this[int x, int y]
        {
            get
            {
                if (x >= 0 && y >= 0 && x < this.TiledMap.Width && y < this.TiledMap.Height)
                {
                    return this.tiles[x, y];
                }

                return null;
            }
        }

        public override void OnAddedToScene()
        {
            this.renderer = new TiledMapRenderer(this.Scene.GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            this.renderer.Update(this.TiledMap, gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var viewMatrix = this.Scene.Camera.GetViewMatrix();
            this.renderer.Draw(this.TiledMap, viewMatrix);
        }
    }

    public class Tile
    {
        public GameMap Map { get; }
        public Vector2 Position { get; }

        public Tile(GameMap map, int x, int y)
        {
            this.Map = map;
            this.Position = new Vector2(x, y);
        }
    }
}
