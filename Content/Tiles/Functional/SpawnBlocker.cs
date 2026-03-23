using System.Collections.Generic;
using System.Security.Policy;
using AutomationPlus.Content.Tiles.Traps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationPlus.Content.Tiles.Functional
{
    public class SpawnBlocker : ConfigurableTile
    {
        public override string Texture => "Terraria/Images/Tiles_0";
        public override void PlaceInWorld(int i, int j, Item item)
        {
            TileEntity.PlaceEntityNet(i, j, ModContent.TileEntityType<SpawnBlockerEntity>());
            ModContent.GetInstance<SpawnBlockSystem>().RegisterSpawnBlockEntity((SpawnBlockerEntity)TileEntity.ByPosition[new Point16(i, j)]);
        }
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;

            AddMapEntry(new Color(200, 50, 50));
        }

        public override void HitWire(int i, int j)
        {
            var tile = Main.tile[i, j];
            tile.TileFrameX = (short)((tile.TileFrameX + 18) % 36);
        }

        public override void OpenConfigUI(int i, int j)
        {
            base.OpenConfigUI(i, j);
        }
    }
    public class SpawnBlockerEntity : ModTileEntity
    {
        public bool ShowRange { get; set; } = true;
        public int RangeX { get; private set; } = 5;
        public int RangeY { get; private set; } = 5;
        public override bool IsTileValidForEntity(int x, int y)
        {
            var tile = Main.tile[x, y];
            return
                tile.TileType == ModContent.TileType<SpawnBlocker>() &&
                tile.TileFrameX == 0 &&
                tile.TileFrameY == 0;
        }

        public override void Update()
        {
            if (!IsTileValidForEntity(Position.X, Position.Y))
            {
                ModContent.GetInstance<SpawnBlockSystem>().DeregisterSpawnBlockEntity(this);
                Kill(Position.X, Position.Y);
            }
        }
    }
    public class SpawnBlockSystem : ModSystem
    {
        private readonly HashSet<int> tileEntityIDs = [];
        private readonly HashSet<Point16> tileEntityPostions = [];
        public bool RegisterSpawnBlockEntity(SpawnBlockerEntity entity)
        {
            return tileEntityIDs.Add(entity.ID) && tileEntityPostions.Add(entity.Position);

        }
        public bool DeregisterSpawnBlockEntity(SpawnBlockerEntity entity)
        {
            return tileEntityIDs.Remove(entity.ID) && tileEntityPostions.Remove(entity.Position);
        }
        public bool TryGetSpawnBlockerEntityAtPosition(Point16 position, out SpawnBlockerEntity entity)
        {
            entity = null;
            if (tileEntityPostions.Contains(position))
            {
                foreach (var id in tileEntityIDs)
                {
                    if (TileEntity.ByID[id] is SpawnBlockerEntity e && e.Position == position)
                    {
                        entity = e;
                        return true;
                    }
                }
            }
            return false;
        }
        public override void PostDrawTiles()
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Matrix.Identity);
            foreach (var i in tileEntityIDs)
            {
                if (TileEntity.ByID[i] is SpawnBlockerEntity entity)
                {
                    if (entity.ShowRange)
                    {
                        var center = entity.Position.ToWorldCoordinates(8, 8) - Main.screenPosition;
                        var width = entity.RangeX * 16;
                        var height = entity.RangeY * 16;
                        var rect = new Rectangle(
                            (int)(center.X - width / 2),
                            (int)(center.Y - height / 2),
                            width,
                            height
                        );
                        Main.spriteBatch.Draw(
                            TextureAssets.MagicPixel.Value,
                            rect,
                            new Color(255, 0, 0, 100)
                        );
                    }
                }
            }
            Main.spriteBatch.End();
        }
    }
}