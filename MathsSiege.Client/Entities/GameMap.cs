using MathsSiege.Client.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MathsSiege.Client.Entities
{
    public class GameMap : DrawableEntity
    {
        private TiledMapRenderer renderer;

        private Tile[,] tiles;
        private List<Tile> spawnableTiles = new List<Tile>();

        public TiledMap TiledMap { get; }
        public IReadOnlyList<Tile> SpawnableTiles => this.spawnableTiles;

        private WallManager wallManager;
        private DefenceManager defenceManager;

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

                            if (this.tiles[x, y].IsSpawnable)
                            {
                                this.spawnableTiles.Add(this.tiles[x, y]);
                            }
                        }
                    }
                }
            }

            // Store references to each tile's adjacent tiles.
            foreach (var tile in this.tiles)
            {
                if (tile.Position.X > 0)
                {
                    tile.AdjacentTiles.Add(this[tile.X - 1, tile.Y]);
                }

                if (tile.Position.X < this.TiledMap.Width - 1)
                {
                    tile.AdjacentTiles.Add(this[tile.X + 1, tile.Y]);
                }

                if (tile.Position.Y > 0)
                {
                    tile.AdjacentTiles.Add(this[tile.X, tile.Y - 1]);
                }

                if (tile.Position.Y < this.TiledMap.Height - 1)
                {
                    tile.AdjacentTiles.Add(this[tile.X, tile.Y + 1]);
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

        /// <summary>
        /// Finds the shortest path between two tiles, using A* pathfinding.
        /// </summary>
        /// <param name="start">The start tile.</param>
        /// <param name="finish">The destination tile.</param>
        /// <param name="shouldAvoidWalls">Whether to avoid walls or not.</param>
        /// <returns></returns>
        public IList<Tile> GetPath(Tile start, Tile finish, bool shouldAvoidWalls = true)
        {
            IList<Tile> path = new List<Tile>();

            List<Tile> open = new List<Tile>();
            List<Tile> closed = new List<Tile>();

            // If the start and finish is the same or the finish
            // is adjacent to the start, return an empty path.
            if (start == finish || start.AdjacentTiles.Contains(finish))
            {
                return path;
            }

            start.Node.G = 0;
            start.Node.H = this.HeuristicCostEstimate(start, finish);

            open.Add(start);

            do
            {
                // Get the tile that is currently the most optimal
                // to go via.
                Tile current = this.GetTileWithLowestF(open);

                // If this is the finish tile, the path has
                // been found.
                if (current == finish)
                {
                    path = this.TracePath(closed);
                    break;
                }

                open.Remove(current);
                closed.Add(current);

                // Loop through all of the tile's adjacent tiles that
                // are valid for moving to.
                foreach (var adjacent in current.AdjacentTiles
                    .Where((tile) => this.IsValidTile(tile, finish, shouldAvoidWalls)))
                {
                    // Ignore if it has already been considered.
                    if (closed.Contains(adjacent))
                    {
                        continue;
                    }

                    // Add it to the tiles to consider if it has
                    // not yet been added.
                    if (!open.Contains(adjacent))
                    {
                        open.Add(adjacent);
                    }

                    // Calculate the distance from the start to
                    // this tile.
                    int g = current.Node.G + 1;

                    // Change the path if the new path is more optimal.
                    if (g < adjacent.Node.G)
                    {
                        adjacent.Node.Previous = current.Node;
                        adjacent.Node.G = g;
                        adjacent.Node.H = this.HeuristicCostEstimate(adjacent, finish);
                    }
                }
            }
            while (open.Any());

            // Reset the values for the next run of the algorithm.
            open.ForEach(tile => tile.Node.Reset());
            closed.ForEach(tile => tile.Node.Reset());

            // If no path was found while avoiding walls, try again
            // without avoiding walls. This way enemies will try to
            // avoid walls if possible, or attack them if not.
            if (path.Count == 0)
            {
                if (shouldAvoidWalls)
                {
                    path = this.GetPath(start, finish, false);
                }
                else
                {
                    path = null;
                }
            }

            return path;
        }

        /// <summary>
        /// Checks if a tile is a valid tile to move to.
        /// </summary>
        /// <param name="tile">The tile to check.</param>
        /// <param name="finish">The finish tile.</param>
        /// <param name="shouldAvoidWalls">Whether to avoid walls.</param>
        /// <returns></returns>
        private bool IsValidTile(Tile tile, Tile finish, bool shouldAvoidWalls)
        {
            return
                // The tile must be walkable
                (tile.IsWalkable)
                // The tile must either be the finish tile or
                // not contain a defence.
                && (!this.defenceManager.CheckContainsDefence(tile) || tile == finish)
                // The tile must not contain a wall if they should
                // be avoided.
                && (!shouldAvoidWalls || !this.wallManager.CheckContainsWall(tile));
        }

        /// <summary>
        /// Calculates an estimate of the cost to travel between two tiles.
        /// </summary>
        /// <param name="from">The start tile.</param>
        /// <param name="to">The destination tile.</param>
        /// <returns></returns>
        private float HeuristicCostEstimate(Tile from, Tile to)
        {
            var displacement = to.Position - from.Position;
            return Math.Abs(displacement.X) + Math.Abs(displacement.Y);
        }

        /// <summary>
        /// Gets the tile with the lowest F value from a list of tiles.
        /// </summary>
        /// <param name="tiles">A list of tiles.</param>
        /// <returns></returns>
        private Tile GetTileWithLowestF(List<Tile> tiles)
        {
            Tile lowest = null;

            foreach (var tile in tiles)
            {
                if (lowest == null || tile.Node.F < lowest.Node.F)
                {
                    lowest = tile;
                }
            }

            return lowest;
        }

        /// <summary>
        /// Traces a path through a list of tiles, using the last
        /// tile in the list as the starting point.
        /// </summary>
        /// <param name="tiles">A list of tiles.</param>
        /// <returns></returns>
        private List<Tile> TracePath(List<Tile> tiles)
        {
            var path = new List<Tile>();
            var current = tiles.Last();

            while (current.Node.Previous != null)
            {
                path.Add(current);
                current = current.Node.Previous.Tile;
            }

            return path;
        }

        public override void OnAddedToScene()
        {
            this.renderer = new TiledMapRenderer(this.Scene.GraphicsDevice);
            this.wallManager = this.Scene.Services.GetService<WallManager>();
            this.defenceManager = this.Scene.Services.GetService<DefenceManager>();
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

        public int X => (int)this.Position.X;
        public int Y => (int)this.Position.Y;

        public bool IsWalkable { get; set; }
        public bool IsPlaceable { get; set; }
        public bool IsSpawnable { get; set; }

        public List<Tile> AdjacentTiles { get; } = new List<Tile>();

        public PathfindingNode Node { get; }

        public Tile(GameMap map, int x, int y)
        {
            this.Map = map;
            this.Position = new Vector2(x, y);
            this.Node = new PathfindingNode(this);
        }

        /// <summary>
        /// Represents a node in the graph to use for pathfinding.
        /// </summary>
        public class PathfindingNode
        {
            private const int MaxGValue = 5000;

            public readonly Tile Tile;

            public int G = MaxGValue;
            public float H;
            public float F => this.G + this.H;

            public PathfindingNode Previous { get; set; }

            public PathfindingNode(Tile tile)
            {
                this.Tile = tile;
            }

            public void Reset()
            {
                this.G = MaxGValue;
                this.H = 0;
                this.Previous = null;
            }
        }
    }
}
