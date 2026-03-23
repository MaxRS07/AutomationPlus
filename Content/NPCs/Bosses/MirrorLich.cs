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
    }
}