using Terraria.ModLoader;
using Terraria;
using AutomationPlus.Content.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using JetBrains.Annotations;
using AutomationPlus.UI;
using AutomationPlus.UI.Elements;
using System;

namespace AutomationPlus.Content.Tiles.Traps
{
    public class LunarDartTrap : ConfigurableTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileWaterDeath[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
        }

        public override void HitWire(int i, int j)
        {
            if (!Wiring.CheckMech(i, j, 120))
            {
                return;
            }

            var tile = Main.tile[i, j];
            var frame = tile.TileFrameX / 18;
            var direction = frame switch
            {
                0 => Vector2.UnitX * -1, // LEFT
                1 => Vector2.UnitX,  // RIGHT
                2 => Vector2.UnitY * -1, // UP
                3 => Vector2.UnitY,  // DOWN
                _ => Vector2.UnitY // DOWN
            };
            var position = new Vector2(i, j).ToWorldCoordinates() + direction * 16;
            var projectileType = ModContent.ProjectileType<LunarDart>();
            Projectile.NewProjectile(new Terraria.DataStructures.EntitySource_Wiring(i, j), position, direction * 15f, projectileType, 160, 4);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            // 38, 255, 239
            r = 0.15f;
            g = 1f;
            b = 0.94f;
        }

        public override void OpenConfigUI(int i, int j)
        {
            var configSystem = ModContent.GetInstance<MultitoolConfigSystem>();
            if (configSystem == null)
            {
                return;
            }

            var worldPosition = new Vector2(i, j) * 16;
            configSystem.SetMainPanelPosition(worldPosition.X, worldPosition.Y);
            configSystem.SetConfiguredTile(i, j);
            configSystem.ShowUI();
        }

        public override bool Slope(int i, int j)
        {
            var tile = Main.tile[i, j];
            var frame = tile.TileFrameX / 18;

            frame = frame switch
            {
                0 => 2,
                2 => 4,
                4 => 1,
                1 => 3,
                3 => 5,
                _ => 0
            };

            tile.TileFrameX = (short)(frame * 18);
            return false;
        }

        public override void OnDirectionSelected(int i, int j, UIArrowButton.Direction direction)
        {
            var tile = Main.tile[i, j];
            if (tile == null || !tile.HasTile)
            {
                return;
            }

            switch (tile.TileFrameX / 18 % 2)
            {
                case 0:
                    tile.TileFrameX = direction switch
                    {
                        UIArrowButton.Direction.Left => 0,
                        UIArrowButton.Direction.Right => 18,
                        UIArrowButton.Direction.Up => 36,
                        UIArrowButton.Direction.Down => 72,
                        _ => tile.TileFrameX
                    };
                    break;
                case 1:
                    tile.TileFrameX = direction switch
                    {
                        UIArrowButton.Direction.Left => 0,
                        UIArrowButton.Direction.Right => 18,
                        UIArrowButton.Direction.Up => 54,
                        UIArrowButton.Direction.Down => 90,
                        _ => tile.TileFrameX
                    };
                    return; // No change needed
            }
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            var dir = Main.LocalPlayer.direction;

            Main.tile[i, j].TileFrameX = (short)(dir == -1 ? 0 : 18);
        }

        public override void MouseOverFar(int i, int j)
        {
            var player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<Items.Tiles.LunarDartTrapItem>();
        }
    }
}