using Terraria.UI;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria.ID;
using AutomationPlus.UI.Elements;
using AutomationPlus.Content.Tiles.Traps;

namespace AutomationPlus.UI
{
    [Autoload(Side = ModSide.Client)]
    public class MultitoolConfigSystem : ModSystem
    {
        internal UserInterface MultitoolConfigInterface;
        internal MultitoolConfigState MultitoolConfigState;
        private readonly HashSet<Point> configuredTiles = new();

        public event System.Action<UIArrowButton.Direction, Point> DirectionChanged;

        public override void PostSetupContent()
        {
            MultitoolConfigInterface = new UserInterface();
            MultitoolConfigState = new MultitoolConfigState();
            MultitoolConfigState.Activate();
            MultitoolConfigInterface.SetState(null);
        }

        public void ShowUI()
        {
            if (MultitoolConfigInterface == null || MultitoolConfigState == null)
            {
                return;
            }

            MultitoolConfigInterface?.SetState(MultitoolConfigState);
        }
        public void HideUI()
        {
            MultitoolConfigInterface?.SetState(null);
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

        public void SetMainPanelPosition(float leftPixels, float topPixels)
        {
            if (MultitoolConfigState == null)
            {
                return;
            }
            MultitoolConfigState.SetMainPanelTargetPosition(leftPixels, topPixels);
        }
        public override void ModifyScreenPosition()
        {
            MultitoolConfigState.ScreenPosition = Main.screenPosition;
        }

        public override void Unload()
        {
            MultitoolConfigInterface = null;
            MultitoolConfigState = null;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "AutomationPlus: Multitool Config",
                    delegate
                    {
                        if (MultitoolConfigInterface?.CurrentState != null)
                        {
                            MultitoolConfigInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.Game)
                );
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (MultitoolConfigInterface?.CurrentState != null)
            {
                MultitoolConfigInterface.Update(gameTime);
            }
        }
    }
}