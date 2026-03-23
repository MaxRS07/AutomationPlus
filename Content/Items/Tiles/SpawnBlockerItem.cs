using AutomationPlus.Content.Tiles.Functional;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationPlus.Content.Items.Tiles
{
    public class SpawnBlockerItem : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.DirtBlock}";
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<SpawnBlocker>();
        }
    }
}