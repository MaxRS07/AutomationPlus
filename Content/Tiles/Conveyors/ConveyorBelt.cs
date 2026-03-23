using System;
using System.Collections.Generic;
using AutomationPlus.Content.Items.Upgrades;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AutomationPlus.Content.Tiles.Conveyors
{
    public abstract class ConveyorBelt : ModTile
    {
        public abstract float GetBeltSpeed();
        public abstract int BeltDirection();
        public abstract ushort ReversedType();

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            TileID.Sets.ConveyorDirection[Type] = BeltDirection();
            TileID.Sets.HasSlopeFrames[Type] = true;
            TileID.Sets.IgnoresNearbyHalfbricksWhenDrawn[Type] = true;
            TileID.Sets.IsSkippedForNPCSpawningGroundTypeCheck[Type] = false;

            DustType = -1;

            AddMapEntry(Color.Gray);
        }
        public override void HitWire(int i, int j)
        {
            var tile = Main.tile[i, j];
            if (!tile.IsActuated)
            {
                tile.TileType = ReversedType();
                WorldGen.SquareTileFrame(i, j, true);
                NetMessage.SendTileSquare(-1, i, j, 1, TileChangeType.None);
            }
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            float speedFactor = GetBeltSpeed() / 5f;
            int frameTime = Math.Max(2, (int)MathF.Ceiling(4f / Math.Max(speedFactor, 0.01f)));
            if (++frameCounter >= frameTime)
            {
                frameCounter = 0;
                if (--frame < 0)
                    frame = 3;
            }
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            var frame = Main.tileFrame[type];
            if (BeltDirection() == 1)
                frame = 3 - frame;
            frameYOffset += frame * 90;
        }
    }
}