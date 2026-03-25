using System.Collections.Generic;
using System.Configuration;
using System.Security.Policy;
using AutomationPlus.Content.Tiles.Traps;
using AutomationPlus.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

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
            var configSystem = ModContent.GetInstance<MultitoolConfigSystem>();
            if (configSystem == null)
            {
                return;
            }

            if (TileEntity.ByPosition[new Point16(i, j)] is not SpawnBlockerEntity myEntity)
            {
                return;
            }

            var screenPosition = new Vector2(i, j) * 16 - Main.screenPosition;
            configSystem.SetMainPanelPosition(screenPosition.X, screenPosition.Y);
            configSystem.SetSpawnBlockView(myEntity.RangeX, myEntity.RangeY, myEntity.ShowRange);
            configSystem.SetConfiguredTile(i, j);
            configSystem.ShowUI();
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly || noItem)
            {
                return;
            }

            var system = ModContent.GetInstance<SpawnBlockSystem>();

            if (TileEntity.ByPosition[new Point16(i, j)] is SpawnBlockerEntity entity)
            {
                ModContent.GetInstance<SpawnBlockSystem>().DeregisterSpawnBlockEntity(entity);
                entity.Kill(i, j);
            }
        }
    }
    public class SpawnBlockerEntity : ModTileEntity
    {
        public bool ShowRange = true;
        public int RangeX = 5;
        public int RangeY = 5;

        private bool isTileOn => Main.tile[Position.X, Position.Y].TileFrameX == 0;
        public override bool IsTileValidForEntity(int x, int y)
        {
            var tile = Main.tile[x, y];
            return
                tile.TileType == ModContent.TileType<SpawnBlocker>() &&
                tile.TileFrameY == 0;
        }

        public override void Update()
        {
            if (!isTileOn)
            {
                ShowRange = false;
            }
        }
        public override void SaveData(TagCompound tag)
        {
            tag["ShowRange"] = ShowRange;
            tag["RangeX"] = RangeX;
            tag["RangeY"] = RangeY;
        }
        public override void LoadData(TagCompound tag)
        {
            ShowRange = tag.GetBool("ShowRange");
            RangeX = tag.GetInt("RangeX");
            RangeY = tag.GetInt("RangeY");
        }
        public override void OnKill()
        {
            var system = ModContent.GetInstance<SpawnBlockSystem>();
            system?.DeregisterSpawnBlockEntity(this);
        }
    }
    public class SpawnBlockSystem : ModSystem
    {
        private readonly HashSet<int> tileEntityIDs = [];
        private readonly HashSet<Point16> tileEntityPostions = [];

        public IEnumerable<Rectangle> GetSpawnBlockerHitboxes()
        {
            foreach (var id in tileEntityIDs)
            {
                if (TileEntity.ByID[id] is SpawnBlockerEntity entity)
                {
                    var width = entity.RangeX;
                    var height = entity.RangeY;

                    var rect = new Rectangle(
                        entity.Position.X,
                        entity.Position.Y,
                        width,
                        height
                    );

                    yield return rect;
                }
            }
        }
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
        public bool NPCIntersectsSpawnBlockers(int x, int y) => NPCIntersectsSpawnBlockers(new Point(x, y));
        public bool NPCIntersectsSpawnBlockers(Point pos)
        {
            foreach (var id in tileEntityIDs)
            {
                if (TileEntity.ByID[id] is SpawnBlockerEntity entity)
                {
                    var rect = new Rectangle(
                        entity.Position.X - entity.RangeX,
                        entity.Position.Y - entity.RangeY,
                        entity.RangeX * 2,
                        entity.RangeY * 2
                    );
                    if (rect.Contains(pos))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override void PostDrawTiles()
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
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
                            (int)(center.X - width),
                            (int)(center.Y - height),
                            width * 2,
                            height * 2
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
    public class SpawnBlockedNPC : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            var spawnBlockSystem = ModContent.GetInstance<SpawnBlockSystem>();
            if (spawnBlockSystem.NPCIntersectsSpawnBlockers(spawnInfo.SpawnTileX, spawnInfo.SpawnTileY))
            {
                pool.Clear();
            }
        }
    }
}