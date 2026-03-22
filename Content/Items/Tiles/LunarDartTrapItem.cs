using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using AutomationPlus.Content.Tiles.Traps;

namespace AutomationPlus.Content.Items.Tiles
{
    public class LunarDartTrapItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = Item.buyPrice(silver: 5);
            Item.rare = ItemRarityID.Green;
            Item.createTile = ModContent.TileType<LunarDartTrap>();
        }
    }
}