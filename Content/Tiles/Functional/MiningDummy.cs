using System.Reflection.Metadata.Ecma335;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AutomationPlus.Content.Tiles.Functional
{
    public class MiningDummy : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(200, 200, 200));
        }

        public override bool PreDrawPlacementPreview(int i, int j, SpriteBatch spriteBatch, ref Rectangle frame, ref Vector2 position, ref Color color, bool validPlacement, ref SpriteEffects spriteEffects)
        {
            var texture = ModContent.Request<Texture2D>("AutomationPlus/Content/Tiles/Functional/MiningDummy").Value;
            var drawPos = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y);
            spriteBatch.Draw(texture, drawPos, Color.White);
            return false;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            return false;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX != 0 && tile.TileFrameY != 0)
            {
                return;
            }
            var texture = ModContent.Request<Texture2D>("AutomationPlus/Content/Tiles/Functional/MiningDummy").Value;
            var drawPos = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y);
            spriteBatch.Draw(texture, drawPos, Color.White);
        }
    }
    public class MiningDummyEntity : ModTileEntity
    {
        public override void OnInventoryDraw(Player player, SpriteBatch spriteBatch)
        {
            base.OnInventoryDraw(player, spriteBatch);
        }
        public override bool IsTileValidForEntity(int x, int y)
        {
            var tile = Main.tile[x, y];
            return
                tile.TileType == ModContent.TileType<MiningDummy>() &&
                tile.TileFrameX == 0 &&
                tile.TileFrameY == 0;
        }
        public override void Update()
        {
            // This is just a dummy tile entity to test mining speed modifiers.
        }
    }
}