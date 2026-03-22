

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationPlus.Content.Tiles
{
    public class AdvancedConveyorBeltCounterClockwise : ConveyorBelt
    {
        public override float GetBeltSpeed() => 7.5f;

        public override int BeltDirection() => -1;

        public override ushort ReversedType() => (ushort)ModContent.TileType<AdvancedConveyorBeltClockwise>();
    }
}