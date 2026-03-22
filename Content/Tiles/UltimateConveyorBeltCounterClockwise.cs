

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationPlus.Content.Tiles
{
    public class UltimateConveyorBeltCounterClockwise : ConveyorBelt
    {
        public override float GetBeltSpeed() => 15f;

        public override int BeltDirection() => -1;

        public override ushort ReversedType() => (ushort)ModContent.TileType<UltimateConveyorBeltClockwise>();
    }
}