using AutomationPlus.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationPlus.Content.Items.Tiles
{
    public class UltimateConveyorBeltClockwiseItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ConveyorBeltRight);
            Item.createTile = ModContent.TileType<UltimateConveyorBeltClockwise>();
        }

        public override void AddRecipes()
        {
            var recipe = Recipe.Create(Type, 5);
            recipe.AddIngredient(ItemID.ConveyorBeltRight);
            recipe.AddIngredient(ItemID.HallowedBar);
            recipe.Register();
        }
    }
}