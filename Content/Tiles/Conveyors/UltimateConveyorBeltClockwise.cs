

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationPlus.Content.Tiles.Conveyors
{
    public class UltimateConveyorBeltClockwise : ConveyorBelt
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override float GetBeltSpeed() => 15f;

        public override int BeltDirection() => 1;

        public override ushort ReversedType() => (ushort)ModContent.TileType<UltimateConveyorBeltCounterClockwise>();
    }
}