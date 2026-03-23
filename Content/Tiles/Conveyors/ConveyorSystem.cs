
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Terraria;
using Terraria.GameContent.Golf;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Physics;

namespace AutomationPlus.Content.Tiles.Conveyors
{
    public class ConveyorSystem : ModSystem
    {
        public override void Load()
        {
            On_Collision.StepConveyorBelt += OnCollisionStepConveyorBelt;
            On_GolfHelper.ContactListener.OnCollision += OnGolfHelperContactListenerOnCollision;
            // GolfHelper chage `private static readonly GolfHelper.ContactListener Listener = new GolfHelper.ContactListener();` to custom listener
        }
        public void OnCollisionStepConveyorBelt(On_Collision.orig_StepConveyorBelt orig, Entity entity, float gravDir)
        {
            Player player = (Player)null;
            if (entity is Player)
            {
                player = (Player)entity;
                if ((double)Math.Abs(player.gfxOffY) > 2.0 || player.grapCount > 0 || player.pulley)
                    return;
                entity.height -= 5;
                entity.position.Y += 5f;
            }
            int num1 = 0;
            int num2 = 0;
            var speed = 2.5f;
            bool flag = false;
            int num3 = (int)entity.position.Y + entity.height;
            entity.Hitbox.Inflate(2, 2);
            Vector2 topLeft = entity.TopLeft;
            Vector2 topRight = entity.TopRight;
            Vector2 bottomLeft = entity.BottomLeft;
            Vector2 bottomRight = entity.BottomRight;
            List<Point> entityEdgeTiles = [];
            Collision.GetEntityEdgeTiles(entityEdgeTiles, entity, false, false, true, true);
            Vector2 vector2_1 = new(0.0001f);
            foreach (Point point in entityEdgeTiles)
            {
                if (WorldGen.InWorld(point.X, point.Y, 0) && (player == null || !player.onTrack || point.Y >= num3))
                {
                    Tile tile = Main.tile[point.X, point.Y];
                    ModTile modTile = ModContent.GetModTile(tile.TileType);
                    if (modTile != null && modTile is ConveyorBelt belt)
                    {
                        speed = belt.GetBeltSpeed();
                    }

                    if (tile != null && !tile.IsActuated)
                    {
                        int num4 = TileID.Sets.ConveyorDirection[tile.TileType];
                        if (num4 != 0)
                        {
                            Vector2 lineStart1;
                            Vector2 lineStart2;
                            lineStart1.X = lineStart2.X = (float)(point.X * 16);
                            Vector2 lineEnd1;
                            Vector2 lineEnd2;
                            lineEnd1.X = lineEnd2.X = (float)(point.X * 16 + 16);
                            switch (tile.Slope)
                            {
                                case (SlopeType)1:
                                    lineStart2.Y = (float)(point.Y * 16);
                                    lineEnd2.Y = lineEnd1.Y = lineStart1.Y = (float)(point.Y * 16 + 16);
                                    break;
                                case (SlopeType)2:
                                    lineEnd2.Y = (float)(point.Y * 16);
                                    lineStart2.Y = lineEnd1.Y = lineStart1.Y = (float)(point.Y * 16 + 16);
                                    break;
                                case (SlopeType)3:
                                    lineEnd1.Y = lineStart2.Y = lineEnd2.Y = (float)(point.Y * 16);
                                    lineStart1.Y = (float)(point.Y * 16 + 16);
                                    break;
                                case (SlopeType)4:
                                    lineStart1.Y = lineStart2.Y = lineEnd2.Y = (float)(point.Y * 16);
                                    lineEnd1.Y = (float)(point.Y * 16 + 16);
                                    break;
                                default:
                                    lineStart2.Y = !tile.IsHalfBlock ? (lineEnd2.Y = (float)(point.Y * 16)) : (lineEnd2.Y = (float)(point.Y * 16 + 8));
                                    lineStart1.Y = lineEnd1.Y = (float)(point.Y * 16 + 16);
                                    break;
                            }
                            int num5 = 0;
                            if (!TileID.Sets.Platforms[tile.TileType] && Collision.CheckAABBvLineCollision2(entity.position - vector2_1, entity.Size + vector2_1 * 2f, lineStart1, lineEnd1))
                                --num5;
                            if (Collision.CheckAABBvLineCollision2(entity.position - vector2_1, entity.Size + vector2_1 * 2f, lineStart2, lineEnd2))
                                ++num5;
                            if (num5 != 0)
                            {
                                flag = true;
                                num1 += num4 * num5 * (int)gravDir;
                                if (tile.Slope == SlopeType.SlopeUpLeft)
                                    num2 += (int)gravDir * -num4;
                                if (tile.Slope == SlopeType.SlopeDownLeft)
                                    num2 -= (int)gravDir * -num4;
                            }
                        }
                    }
                }
            }
            if (entity is Player)
            {
                entity.height += 5;
                entity.position.Y -= 5f;
            }
            if (!flag || num1 == 0)
                return;
            int num6 = Math.Sign(num1);
            int num7 = Math.Sign(num2);
            Vector2 Velocity = Vector2.Normalize(new Vector2((float)num6 * gravDir, (float)num7)) * speed;
            Vector2 vector2_2 = Collision.TileCollision(entity.position, Velocity, entity.width, entity.height, false, false, (int)gravDir);
            entity.position += vector2_2;
            Velocity = new Vector2(0.0f, 2.5f * gravDir);
            Vector2 vector2_3 = Collision.TileCollision(entity.position, Velocity, entity.width, entity.height, false, false, (int)gravDir);
            entity.position += vector2_3;
        }

        public void OnGolfHelperContactListenerOnCollision(On_GolfHelper.ContactListener.orig_OnCollision orig, GolfHelper.ContactListener self, PhysicsProperties properties, ref Vector2 position, ref Vector2 velocity, ref BallCollisionEvent collision)
        {
            TileMaterial byTileId = TileMaterials.GetByTileId(collision.Tile.TileType);
            Vector2 vector2_1 = velocity * byTileId.GolfPhysics.SideImpactDampening;
            Vector2 vector2_2 = collision.Normal * Vector2.Dot(velocity, collision.Normal) * (byTileId.GolfPhysics.DirectImpactDampening - byTileId.GolfPhysics.SideImpactDampening);
            velocity = vector2_1 + vector2_2;
            Projectile entity = collision.Entity as Projectile;
            switch (collision.Tile.TileType)
            {
                case TileID.ConveyorBeltLeft:
                case TileID.ConveyorBeltRight:
                case ushort type when ConveyorHelper.IsConveyor(type):
                    var speed = 2.5f;
                    var modTile = ModContent.GetModTile(collision.Tile.TileType);
                    if (modTile != null && modTile is ConveyorBelt belt)
                    {
                        speed = belt.GetBeltSpeed();
                    }
                    float num1 = speed * collision.TimeScale;
                    Vector2 vector2_3 = new(-collision.Normal.Y, collision.Normal.X);
                    if (collision.Tile.TileType == (ushort)422)
                        vector2_3 = -vector2_3;
                    float num2 = Vector2.Dot(velocity, vector2_3);
                    if ((double)num2 < (double)num1)
                    {
                        velocity += vector2_3 * MathHelper.Clamp(num1 - num2, 0.0f, num1 * 0.5f);
                        break;
                    }
                    break;
                case TileID.GolfHole:
                    float num3 = velocity.Length() / collision.TimeScale;
                    if ((double)collision.Normal.Y <= -0.00999999977648258 && (double)num3 <= 100.0)
                    {
                        velocity *= 0.0f;
                        if (entity != null && entity.active)
                        {
                            self.PutBallInCup(entity, collision);
                            break;
                        }
                        break;
                    }
                    break;
            }
            if (entity == null || (double)velocity.Y >= -0.300000011920929 || ((double)velocity.Y <= -2.0 || (double)velocity.Length() <= 1.0))
                return;
            Dust dust = Dust.NewDustPerfect(collision.Entity.Center, DustID.Smoke, new Vector2?(collision.Normal), (int)sbyte.MaxValue, new Color(), 1f);
            dust.scale = 0.7f;
            dust.fadeIn = 1f;
            dust.velocity = dust.velocity * 0.5f + Main.rand.NextVector2CircularEdge(0.5f, 0.4f);
        }
    }
}