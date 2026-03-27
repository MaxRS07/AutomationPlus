using Terraria.UI;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using AutomationPlus.UI.Elements;
using AutomationPlus.Content.Tiles.Traps;
using AutomationPlus.Content.Tiles.Functional;
using Terraria.DataStructures;
using Microsoft.CodeAnalysis;

namespace AutomationPlus.UI
{
    [Autoload(Side = ModSide.Client)]
    public class MultihammerConfigSystem : ModSystem
    {
        internal UserInterface MultihammerConfigInterface;
        internal MultihammerConfigState MultihammerConfigState;
        private readonly HashSet<Point> configuredTiles = new();

        public event System.Action<UIArrowButton.Direction, Point> DirectionChanged;

        public override void PostSetupContent()
        {
            MultihammerConfigInterface = new UserInterface();
            MultihammerConfigState = new MultihammerConfigState();
            MultihammerConfigState.Activate();
            MultihammerConfigInterface.SetState(null);
        }

        public void ShowUI()
        {
            if (MultihammerConfigInterface == null || MultihammerConfigState == null)
            {
                return;
            }

            MultihammerConfigInterface?.SetState(MultihammerConfigState);
        }
        public void HideUI()
        {
            MultihammerConfigInterface?.SetState(null);
            UISliderBetter.ResetInteractionState();
            configuredTiles.Clear();
        }

        public void SetConfiguredTile(int i, int j)
        {
            configuredTiles.Add(new Point(i, j));
        }

        public void NotifyDirectionChanged(UIArrowButton.Direction direction)
        {
            if (configuredTiles.Count == 0)
            {
                return;
            }

            int changedCount = 0;
            foreach (var tilePosition in configuredTiles)
            {
                var tile = Main.tile[tilePosition.X, tilePosition.Y];
                if (tile == null || !tile.HasTile)
                {
                    continue;
                }

                var modTile = TileLoader.GetTile(tile.TileType);
                if (modTile is not ConfigurableTile configurableTile)
                {
                    continue;
                }

                configurableTile.OnDirectionSelected(tilePosition.X, tilePosition.Y, direction);
                changedCount++;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendTileSquare(Main.myPlayer, tilePosition.X, tilePosition.Y, 1, TileChangeType.None);
                }

                DirectionChanged?.Invoke(direction, tilePosition);
            }
        }

        public void NotifySpawn(bool? enabled, int? rangeX, int? rangeY)
        {
            if (configuredTiles.Count == 0)
            {
                return;
            }

            var spawnBlockSystem = ModContent.GetInstance<SpawnBlockSystem>();
            foreach (var tilePosition in configuredTiles)
            {
                var point16 = new Point16(tilePosition.X, tilePosition.Y);
                if (spawnBlockSystem.TryGetSpawnBlockerEntityAtPosition(point16, out var entity))
                {
                    entity.ShowRange = enabled ?? entity.ShowRange;
                    entity.RangeX = rangeX ?? entity.RangeX;
                    entity.RangeY = rangeY ?? entity.RangeY;
                }
            }
        }

        public void SetMainPanelPosition(float leftPixels, float topPixels)
        {
            if (MultihammerConfigState == null)
            {
                return;
            }
            MultihammerConfigState.SetMainPanelTargetPosition(leftPixels, topPixels);
        }
        public override void ModifyScreenPosition()
        {
            MultihammerConfigState.ScreenPosition = Main.screenPosition;
        }

        public override void Unload()
        {
            MultihammerConfigInterface = null;
            MultihammerConfigState = null;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "AutomationPlus: Multihammer Config",
                    delegate
                    {
                        if (MultihammerConfigInterface?.CurrentState != null)
                        {
                            MultihammerConfigInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI
                    )
                );
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (MultihammerConfigInterface?.CurrentState != null)
            {
                MultihammerConfigInterface.Update(gameTime);
            }
        }

        public void SetDirectionalView()
        {
            MultihammerConfigState?.SetDirectionalView();
        }
        public void SetSpawnBlockView(int rangeX, int rangeY, bool showRange)
        {
            MultihammerConfigState?.SetSpawnBlockView(rangeX, rangeY, showRange);
        }
    }
}