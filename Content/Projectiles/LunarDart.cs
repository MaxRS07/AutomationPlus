using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Build.Construction;
namespace AutomationPlus.Content.Projectiles
{
    public class LunarDart : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.PoisonDartBlowgun}"; //placeholder
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.PoisonDartBlowgun);
            Projectile.Opacity = 1;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            // Add some light and dust effects for visual flair
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 1f);
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}