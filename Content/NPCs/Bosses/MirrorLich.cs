using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace AutomationPlus.Content.NPCs.Bosses
{
    public class MirrorLich : ModNPC
    {
        public override string Texture => $"Terraria/Images/NPC_{NPCID.Zombie}"; // Placeholder texture
        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 60;
            NPC.damage = 50;
            NPC.defense = 20;
            NPC.lifeMax = 5000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = Item.buyPrice(gold: 10);
            NPC.knockBackResist = 0.5f;
            NPC.aiStyle = -1; // Custom AI
        }

        public override void AI()
        {
            // Basic AI: Move towards the player and attack
            Player target = Main.player[NPC.target];
            if (target.active && !target.dead)
            {
                Vector2 direction = target.Center - NPC.Center;
                direction.Normalize();
                direction *= 3f; // Move speed

                NPC.velocity = (NPC.velocity * 20f + direction) / 21f; // Smooth movement

                // Attack logic (e.g., shoot projectiles every few seconds)
                if (Main.rand.NextBool(120)) // Roughly every 2 seconds
                {
                    int projectileType = ModContent.ProjectileType<Content.Projectiles.LunarDart>();
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, direction * 5f, projectileType, 20, 2);
                }
            }
        }
    }
}