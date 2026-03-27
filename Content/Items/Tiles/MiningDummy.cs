using Terraria.ID;
using Terraria.ModLoader;
using AutomationPlus.Content.Tiles.Functional;

namespace AutomationPlus.Content.Items.Tiles
{
    public class MiningDummyItem : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.TargetDummy}";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mining Dummy");
            // Tooltip.SetDefault("A dummy tile used for testing mining speed modifiers.");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 48;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 0;
            Item.createTile = ModContent.TileType<MiningDummy>();
        }
    }
}