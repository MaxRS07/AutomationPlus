using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using System;

namespace AutomationPlus.Content.Projectiles
{
    public class LunarBeam : ModProjectile
    {
        private float BeamWidth
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private float DamageModifier
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        const int AnimationTime = 15;
        const int MaxBeamLength = 1600;
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.PhantasmalDeathray}"; //placeholder
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.LastPrismLaser);
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 70;
            Projectile.trap = true;
            Projectile.scale = 0.25f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }
        public override void OnSpawn(IEntitySource source)
        {
            DamageModifier = 1;
            Projectile.Center -= Projectile.velocity.SafeNormalize(Vector2.Zero) * 8f;
        }
        public override void AI()
        {
            Projectile.frameCounter++;

            if (Projectile.frameCounter - 30 <= AnimationTime)
            {
                var realCounter = Projectile.frameCounter - 30;
                BeamWidth = MathF.Min(realCounter / (float)AnimationTime, 1f);
            }
            else if (Projectile.timeLeft <= AnimationTime)
            {
                BeamWidth = MathF.Max(Projectile.timeLeft / (float)AnimationTime, 0f);
            }
            else
            {
                BeamWidth = 1;
            }
            BeamWidth = Math.Clamp(BeamWidth, 0f, 1f);

            // Add some light and dust effects for visual flair
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 1f);
            if (Main.rand.NextBool(3))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric);
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float beamLength = PerformBeamHitscan();
            if (beamLength <= 4f)
            {
                return false;
            }

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitY);
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Rectangle endSource = new Rectangle(0, 0, texture.Width, 12);
            Rectangle midSource = new Rectangle(0, 12, texture.Width, 2);

            Vector2 endOrigin = new Vector2(endSource.Width * 0.5f, 0f);
            Vector2 midOrigin = new Vector2(midSource.Width * 0.5f, 0f);

            float rotation = direction.ToRotation() - MathHelper.PiOver2;

            Vector2 scale = new Vector2(BeamWidth * Projectile.scale, Projectile.scale);

            Vector2 beamStart = Projectile.Center;

            var beamColor = new Color(0, 248, 255);

            if (Projectile.frameCounter < 30)
            {
                Main.EntitySpriteDraw(
                    TextureAssets.MagicPixel.Value,
                    beamStart - Main.screenPosition,
                    new Rectangle(0, 0, 1, 1),
                    beamColor,
                    direction.ToRotation(),
                    new Vector2(0f, 0.5f),
                    new Vector2(beamLength, 1f),
                    SpriteEffects.None,
                    0
                );
                return false;
            }

            Main.EntitySpriteDraw(
                texture,
                beamStart - Main.screenPosition,
                endSource,
                lightColor,
                rotation,
                endOrigin,
                scale,
                SpriteEffects.None,
                0
            );

            float drawnLength = endSource.Height * Projectile.scale;
            float endCapStart = beamLength - endSource.Height * Projectile.scale;
            float midStep = midSource.Height * Projectile.scale;

            while (drawnLength < endCapStart)
            {
                Vector2 midPosition = beamStart + direction * drawnLength - Main.screenPosition;
                Main.EntitySpriteDraw(
                    texture,
                    midPosition,
                    midSource,
                    lightColor,
                    rotation,
                    midOrigin,
                    scale,
                    SpriteEffects.None,
                    0
                );

                drawnLength += midStep;
            }

            Vector2 beamEnd = beamStart + direction * beamLength - Main.screenPosition;
            Main.EntitySpriteDraw(
                texture,
                beamEnd,
                endSource,
                lightColor,
                rotation + MathHelper.Pi,
                endOrigin,
                scale,
                SpriteEffects.None,
                0
            );

            return false;
        }

        private float PerformBeamHitscan()
        {
            float[] laserScanResults = new float[3];
            var dir = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 samplingPoint = Projectile.Center + dir * 8f;
            Collision.LaserScan(samplingPoint, dir, 8, MaxBeamLength, laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= 3;

            return averageLengthSample;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.frameCounter < 30)
            {
                return false;
            }
            float dist = PerformBeamHitscan();
            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                Projectile.Center,
                Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * dist,
                8f,
                ref Projectile.localAI[0]
            );
        }

        public override bool ShouldUpdatePosition() => false;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            DamageModifier *= 0.95f;
            modifiers.FinalDamage *= DamageModifier;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            DamageModifier *= 0.95f;
            modifiers.FinalDamage *= DamageModifier;
        }
    }
}