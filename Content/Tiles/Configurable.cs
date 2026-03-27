using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using AutomationPlus.Content.Items;
using AutomationPlus.UI;
using AutomationPlus.UI.Elements;
using Microsoft.Xna.Framework;
using log4net.Util;
using Terraria.GameContent.ObjectInteractions;

namespace AutomationPlus.Content.Tiles.Traps
{
    public abstract class ConfigurableTile : ModTile
    {
        public override bool RightClick(int i, int j)
        {
            var multihammerType = ModContent.ItemType<Multihammer>();
            var player = Main.LocalPlayer;

            if (player.HeldItem.type == multihammerType)
            {
                OpenConfigUI(i, j);
            }

            return base.RightClick(i, j);
        }
        public virtual void OpenConfigUI(int i, int j)
        {
            // Default implementation - can be overridden by derived classes
        }

        public virtual void OnDirectionSelected(int i, int j, UIArrowButton.Direction direction)
        {
            // Default implementation - can be overridden by derived classes
        }
        public override void MouseOverFar(int i, int j)
        {
            var player = Main.LocalPlayer;
            if (player.HeldItem.type != ModContent.ItemType<Multihammer>())
            {
                return;
            }
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            // player.cursorItemIconID = ModContent.ItemType<LunarDartTrapItem>();
        }
    }
}