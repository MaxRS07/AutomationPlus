using Terraria.ModLoader;
using Terraria;
using AutomationPlus.Content.Projectiles;
using Microsoft.Xna.Framework;
using AutomationPlus.UI.Elements;

namespace AutomationPlus.Content.Tiles.Traps
{
    public class LunarDartTrap : ConfigurableTrap
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;

            base.SetStaticDefaults();
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            // 38, 255, 239
            r = 0.15f;
            g = 1f;
            b = 0.94f;
        }

        protected override int ShootCooldown() => 320;

        protected override int ShootDamage() => 75;

        protected override float ShootKnockback() => 4f;

        protected override float ShootSpeed() => 15f;

        protected override int ShootType() => ModContent.ProjectileType<LunarBeam>();
    }
}