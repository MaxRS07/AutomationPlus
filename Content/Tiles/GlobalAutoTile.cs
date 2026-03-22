using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Physics;

namespace AutomationPlus.Content.Tiles
{
    public class GlobalAutoTile : GlobalTile
    {
        public class AutomationGolfHelper : IBallContactListener
        {
            public void OnCollision(PhysicsProperties properties, ref Vector2 position, ref Vector2 velocity, ref BallCollisionEvent collision)
            {
                var tile = TileLoader.GetTile(collision.Tile.TileType);
                if (tile == null)
                {
                    return;
                }
                float num1 = 2.5f * collision.TimeScale;
                Vector2 vector2_3 = new(-collision.Normal.Y, collision.Normal.X);
                float num2 = Vector2.Dot(velocity, vector2_3);
                if ((double)num2 < (double)num1)
                {
                    velocity += vector2_3 * MathHelper.Clamp(num1 - num2, 0.0f, num1 * 0.5f);
                }
            }

            public void OnPassThrough(PhysicsProperties properties, ref Vector2 position, ref Vector2 velocity, ref float angularVelocity, ref BallPassThroughEvent passThrough)
            {
                return;
            }
        }
    }
}