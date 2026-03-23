using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationPlus.Content.Items.Upgrades
{
    public class ConveyorUpgradeI : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.useAnimation = ItemUseStyleID.Swing;
        }

        public override void AddRecipes()
        {
            var recipe = Recipe.Create(Type, 5);
            recipe.AddIngredient(ItemID.HallowedBar);
            recipe.Register();
        }
    }
}