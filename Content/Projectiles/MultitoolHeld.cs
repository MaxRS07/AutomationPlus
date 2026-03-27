using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System.Security.AccessControl;
using AutomationPlus.Content.Items;
using Microsoft.Xna.Framework;

namespace AutomationPlus.Content.Projectiles
{
    public class MultihammerHeld : ModProjectile
    {
        public override string Texture => "Terraria/Images/Item_1";
        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void AI()
        {
            if (!Projectile.TryGetOwner(out Player player))
            {
                return;
            }

            if (player.HeldItem.type == ModContent.ItemType<Multihammer>())
            {
                Projectile.timeLeft = 2;
            }

            Projectile.Center = player.Center;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}