
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AutomationPlus.Content.Tiles
{
    public class ConveyorSystem : ModSystem
    {
        public override void Load()
        {
            On_Collision.StepConveyorBelt += OnCollisionStepConveyorBelt;
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
    }
}