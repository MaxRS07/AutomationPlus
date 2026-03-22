using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationPlus.Content.Tiles
{
    public class GlobalConveyor : GlobalTile
    {
        public override void RightClick(int i, int j, int type)
        {
            ushort _type = (ushort)type;
            if (Main.LocalPlayer.HeldItem.type == ConveyorHelper.GetUpgradeItem(_type))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    var adjacent = ConveyorHelper.ClosestAdjacentTiles(i, j);
                    foreach (var point in adjacent)
                    {
                        if (ConveyorHelper.UpgradeConveyor(point.X, point.Y))
                        {
                            SpawnUpgradeDust(point.X, point.Y);
                            Main.LocalPlayer.ConsumeItem(ConveyorHelper.GetUpgradeItem(_type));
                        }
                    }
                    return;
                }
                if (ConveyorHelper.UpgradeConveyor(i, j))
                {
                    SpawnUpgradeDust(i, j);
                    Main.LocalPlayer.ConsumeItem(ConveyorHelper.GetUpgradeItem(_type));
                    return;
                }
            }
            return;
        }
        private void SpawnUpgradeDust(int i, int j)
        {
            for (int k = 0; k < 10; k++)
            {
                var position = new Vector2(i, j) * 16f;
                Dust.NewDust(position, 16, 16, DustID.SteampunkSteam);
            }
        }
    }
}