using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using System.Linq;

namespace MathsSiege.Client.Entities
{
    public class GameMap : DrawableEntity
    {
        private TiledMapRenderer renderer;

        private Tile[,] tiles;

        public TiledMap TiledMap { get; }

        /// <summary>
        /// The currently hovered tile.
        /// </summary>
        public Tile HoveredTile { get; private set; }

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

                        layer.TryGetTile(x, y, out TiledMapTile? tiledMapTile);

                        TiledMapTile layerTile = tiledMapTile.Value;

                        if (layerTile.IsBlank)
                        {
                            continue;
                        }

                        TiledMapTileset tileset = this.TiledMap.GetTilesetByTileGlobalIdentifier(layerTile.GlobalIdentifier);
                        TiledMapTilesetTile tilesetTile = tileset.Tiles.FirstOrDefault(
                            t => t.LocalTileIdentifier == layerTile.GlobalIdentifier - tileset.FirstGlobalIdentifier);

                        if (tilesetTile != null)
                        {
                            tilesetTile.Properties.TryGetValue("IsWalkable", out string isWalkable);
                            tilesetTile.Properties.TryGetValue("IsPlaceable", out string isPlaceable);
                            tilesetTile.Properties.TryGetValue("IsSpawnable", out string isSpawnable);

                            this.tiles[x, y].IsWalkable = isWalkable == "true";
                            this.tiles[x, y].IsPlaceable = isPlaceable == "true";
                            this.tiles[x, y].IsSpawnable = isSpawnable == "true";
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

        /// <summary>
        /// Gets the map coordinates of the specified screen position.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public Vector2 ScreenToMap(Vector2 coordinates)
        {
            var tileWidthHalf = this.TiledMap.TileWidth / 2f;
            var tileHeightHalf = this.TiledMap.TileHeight / 2f;

            return new Vector2
            {
                X = (coordinates.X / tileWidthHalf + coordinates.Y / tileHeightHalf) / 2,
                Y = (coordinates.Y / tileHeightHalf - (coordinates.X / tileWidthHalf)) / 2
            };
        }

        /// <summary>
        /// Gets the screen position of the specified map coordinates.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public Vector2 MapToScreen(Vector2 coordinates)
        {
            var tileWidthHalf = this.TiledMap.TileWidth / 2f;
            var tileHeightHalf = this.TiledMap.TileHeight / 2f;

            return new Vector2
            {
                X = (coordinates.X - coordinates.Y) * tileWidthHalf,
                Y = (coordinates.X + coordinates.Y) * tileHeightHalf
            };
        }

        public override void OnAddedToScene()
        {
            this.renderer = new TiledMapRenderer(this.Scene.GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            this.renderer.Update(this.TiledMap, gameTime);

            var mouseState = Mouse.GetState();
            var worldPosition = this.Scene.Camera.ScreenToWorld(mouseState.Position.ToVector2());
            var tilePosition = this.ScreenToMap(worldPosition);
            this.HoveredTile = this[(int)tilePosition.X, (int)tilePosition.Y];
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

        public bool IsWalkable { get; set; }
        public bool IsPlaceable { get; set; }
        public bool IsSpawnable { get; set; }

        public Tile(GameMap map, int x, int y)
        {
            this.Map = map;
            this.Position = new Vector2(x, y);
        }
    }
}
