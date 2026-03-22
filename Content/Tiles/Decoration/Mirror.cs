using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ObjectData;
using Terraria.Audio;
using Terraria.DataStructures;
using System.Collections.Generic;
using System.Linq;

namespace AutomationPlus.Content.Tiles.Decoration
{
    public abstract class Mirror : ModTile
    {
        protected int mirrorWidth;
        protected int mirrorHeight;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;

            TileObjectData.newTile.Width = mirrorWidth;
            TileObjectData.newTile.Height = mirrorHeight;
            TileObjectData.newTile.Origin = new Point16(
                mirrorWidth / 2,
                mirrorHeight / 2
            );
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = [
                .. Enumerable.Repeat(16, mirrorHeight)
            ];
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.LavaDeath = true;

            TileObjectData.addTile(Type);

            DustType = DustID.Glass;

            AddMapEntry(new Color(200, 200, 200));
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            var rect = new Rectangle(i * 16, j * 16, mirrorWidth * 16, mirrorHeight * 16);
            ModContent.GetInstance<MirrorSystem>().DrawMirror(spriteBatch, rect);
            return true;
        }
    }
    public class Mirror2x3 : Mirror
    {
        public override void Load()
        {
            mirrorWidth = 2;
            mirrorHeight = 3;
        }
    }
    public class Mirror3x2 : Mirror
    {
        public override void Load()
        {
            mirrorWidth = 3;
            mirrorHeight = 2;
        }
    }
}