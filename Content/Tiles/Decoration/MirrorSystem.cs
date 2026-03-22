using System.Linq;
using AutomationPlus.Content.Items.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace AutomationPlus.Content.Tiles.Decoration
{
    public class MirrorSystem : ModSystem
    {
        private RenderTarget2D mirrorRenderTarget;
        private bool needsRender;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            Main.QueueMainThreadAction(() =>
            {
                if (Main.instance?.GraphicsDevice == null)
                    return;

                mirrorRenderTarget = new RenderTarget2D(
                    Main.instance.GraphicsDevice,
                    Main.screenWidth,
                    Main.screenHeight
                );
            });
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            Main.QueueMainThreadAction(() =>
            {
                mirrorRenderTarget?.Dispose();
                mirrorRenderTarget = null;
            });
        }
        private void RenderMirror(SpriteBatch spriteBatch)
        {
            var targets = spriteBatch.GraphicsDevice.GetRenderTargets();

            var newBatch = new SpriteBatch(Main.instance.GraphicsDevice);

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                var npc = Main.npc[i];
                if (npc.active)
                {
                    Main.instance.DrawNPCDirect(
                        spriteBatch,
                        npc,
                        false,
                        Main.screenPosition
                    );
                }
            }
            spriteBatch.GraphicsDevice.SetRenderTargets(targets);
        }

        // Called by your tile to draw the mirror texture
        public void DrawMirror(SpriteBatch spriteBatch, Rectangle destRect)
        {
            if (Main.dedServ || mirrorRenderTarget == null)
                return;

            if (needsRender)
            {
                RenderMirror(spriteBatch);
                needsRender = false;
            }

            destRect.Location -= Main.screenPosition.ToPoint(); // Adjust for screen position

            spriteBatch.Draw(mirrorRenderTarget, destRect, Color.White);
        }

        public override void PostUpdateNPCs()
        {
            needsRender = true;
        }
    }
}