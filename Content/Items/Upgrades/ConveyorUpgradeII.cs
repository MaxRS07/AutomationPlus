using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationPlus.Content.Items.Upgrades
{
    public class ConveyorUpgradeII : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.useAnimation = ItemUseStyleID.Swing;
        }
    }
}