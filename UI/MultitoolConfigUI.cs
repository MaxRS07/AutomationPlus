using Terraria.UI;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.ModLoader.UI;
using AutomationPlus.UI.Elements;
using Terraria.GameContent;

namespace AutomationPlus.UI
{
    public class MultitoolConfigState : UIState
    {
        public UIPanel MainPanel;

        public UIArrowButton[] DirectionalButtons = new UIArrowButton[4];
        public Vector2 ScreenPosition = new();
        private Vector2 MainPanelTargetPosition = new();

        public void SetMainPanelTargetPosition(float leftPixels, float topPixels)
        {
            MainPanelTargetPosition = new Vector2(leftPixels, topPixels);
        }
        public void ResetView()
        {
            MainPanel.RemoveAllChildren();
        }
        public void SetDirectionalView()
        {
            var text = new UIText("Direction");
            text.Left.Set(4, 0f);
            MainPanel.Append(text);
            for (int i = 0; i < 4; i++)
            {
                var direction = (UIArrowButton.Direction)i;
                var button = new UIArrowButton(direction);
                button.Left.Set(4, i * 0.25f);
                button.OnChangeDirection += (newDir) =>
                {
                    var sys = ModContent.GetInstance<MultitoolConfigSystem>();
                    if (sys == null)
                    {
                        return;
                    }
                    sys.NotifyDirectionChanged(newDir);
                    sys.HideUI();
                };
                DirectionalButtons[i] = button;
                MainPanel.Append(button);
            }
        }

        public void SetSpawnBlockView()
        {
            var text = new UIText("Spawn Blocker");
            text.Left.Set(4, 0f);
            MainPanel.Append(text);

            var toggle = new UIToggleImage(
                TextureAssets.Buff[9], 50, 50, new(), new()
            );
            toggle.Left.Set(4, 0.25f);
            toggle.OnLeftClick += (evt, element) =>
            {
                var sys = ModContent.GetInstance<MultitoolConfigSystem>();
                if (sys != null)
                {
                    sys.NotifySpawnToggle(toggle.IsOn);
                }
            };
            MainPanel.Append(toggle);
        }

        // This class will be the basis for the configuration UI of all traps. It can be extended with specific controls for each trap type.
        public override void OnInitialize()
        {
            MainPanel = new UIPanel();
            MainPanel.SetPadding(6);
            MainPanel.Width.Set(100, 0f);
            MainPanel.Height.Set(25, 0f);

            Append(MainPanel);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (MainPanel == null)
            {
                return;
            }
            var realPosition = MainPanelTargetPosition + Vector2.UnitY * 20f - ScreenPosition; // Adjust for tile size and panel padding

            MainPanel.Left.Set(realPosition.X, 0f);
            MainPanel.Top.Set(realPosition.Y, 0f);
            MainPanel.Recalculate();

            if (Main.mouseLeft && !MainPanel.IsMouseHovering)
            {
                var sys = ModContent.GetInstance<MultitoolConfigSystem>();
                if (sys != null)
                {
                    sys.HideUI();
                }
            }
        }
    }
}