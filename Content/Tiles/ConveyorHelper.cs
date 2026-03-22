using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using AutomationPlus.Content.Items.Upgrades;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationPlus.Content.Tiles
{
    public static class ConveyorHelper
    {
        public static List<Point> ClosestAdjacentTiles(int i, int j, byte maxTiles = byte.MaxValue)
        {
            var start = new Point(i, j);
            var startType = Main.tile[start].TileType;

            HashSet<Point> visited = new(maxTiles) { start };
            Queue<Point> queue = new([start]);
            List<Point> order = new(maxTiles);

            while (queue.Count > 0 && order.Count < maxTiles)
            {
                var currentPoint = queue.Dequeue();
                order.Add(currentPoint);

                foreach (var neighbor in currentPoint.AdjacentPoints())
                {
                    var neighborTile = Main.tile[neighbor];
                    if (neighborTile.TileType != startType)
                        continue;
                    if (visited.Add(neighbor))
                    {
                        if (visited.Count <= maxTiles)
                        {
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }
            return order;
        }


        public static List<Point> AdjacentPoints(this Point p)
        {
            List<Point> adjacentPoints = [];

            for (int y = p.Y - 1; y <= p.Y + 1; y++)
            {
                for (int x = p.X - 1; x <= p.X + 1; x++)
                {
                    if (x == p.X && y == p.Y)
                        continue;

                    if (WorldGen.InWorld(x, y, 0))
                        adjacentPoints.Add(new Point(x, y));
                }
            }

            return adjacentPoints;
        }

        public static bool IsConveyor(this Tile tile) => IsConveyor(tile.TileType);

        public static bool IsConveyor(int type)
        {
            return (
                type == TileID.ConveyorBeltLeft ||
                type == TileID.ConveyorBeltRight ||
                type == ModContent.TileType<AdvancedConveyorBeltClockwise>() ||
                type == ModContent.TileType<AdvancedConveyorBeltCounterClockwise>()
            );
        }

        public static bool UpgradeConveyor(int i, int j)
        {
            var tile = Main.tile[i, j];
            if (!tile.IsConveyor())
            {
                return false;
            }
            var upgradedType = GetUpgradedType(tile.TileType);
            if (upgradedType == tile.TileType)
                return false;

            tile.TileType = upgradedType;
            WorldGen.SquareTileFrame(i, j, true);
            NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
            return true;
        }
        public static ushort GetUpgradedType(ushort type)
        {
            if (type == TileID.ConveyorBeltRight)
            {
                return (ushort)ModContent.TileType<AdvancedConveyorBeltClockwise>();
            }
            if (type == TileID.ConveyorBeltLeft)
            {
                return (ushort)ModContent.TileType<AdvancedConveyorBeltCounterClockwise>();
            }
            return type;
        }
        public static int GetUpgradeItem(ushort type)
        {
            if (type == TileID.ConveyorBeltRight || type == TileID.ConveyorBeltLeft)
            {
                return ModContent.ItemType<ConveyorUpgradeI>();
            }
            return 0;
        }
    }
}