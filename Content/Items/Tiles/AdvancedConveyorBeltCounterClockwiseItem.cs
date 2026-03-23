using AutomationPlus.Content.Tiles.Conveyors;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationPlus.Content.Items.Tiles
{
    public class AdvancedConveyorBeltCounterClockwiseItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ConveyorBeltLeft);
            Item.createTile = ModContent.TileType<AdvancedConveyorBeltCounterClockwise>();
        }
        public override void AddRecipes()
        {
            var recipe = Recipe.Create(Type, 5);
            recipe.AddIngredient(ItemID.ConveyorBeltLeft);
            recipe.AddIngredient(ItemID.HallowedBar);
            recipe.Register();
        }
    }
}