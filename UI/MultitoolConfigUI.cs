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
using Terraria.Localization;
using Microsoft.CodeAnalysis;

namespace AutomationPlus.UI
{
    public class MultitoolConfigState : UIState
    {
        public UIPanel MainPanel;
        public Vector2 ScreenPosition = new();

        public void SetMainPanelTargetPosition(float leftPixels, float topPixels)
        {
            MainPanel.Top.Set(topPixels, 0f);
            MainPanel.Left.Set(leftPixels, 0f);
            MainPanel.Recalculate();
        }
        public void ResetView()
        {
            MainPanel.RemoveAllChildren();
        }
        public void SetDirectionalView()
        {
            ResetView();
            MainPanel.Height.Set(50, 0f);
            var text = new UIText("Direction");
            text.Left.Set(4, 0f);
            MainPanel.Append(text);
            for (int i = 0; i < 4; i++)
            {
                var direction = (UIArrowButton.Direction)i;
                var button = new UIArrowButton(direction);
                button.Left.Set(4, i * 0.25f);
                button.Top.Set(0, 0.5f);
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
                MainPanel.Append(button);
            }
        }

        public void SetSpawnBlockView(int rangeX, int rangeY, bool showRange)
        {
            ResetView();
            MainPanel.Height.Set(60, 0f);
            MainPanel.Width.Set(140, 0f);

            var text = new UIText("Spawn Blocker");
            text.Left.Set(4, 0f);
            MainPanel.Append(text);

            var heightSlider = new UISliderBetter(
                () => MathHelper.Clamp((rangeY - 1) / 63f, 0f, 1f),
                (value) =>
                {
                    float normalized = MathHelper.Clamp(value, 0f, 1f);
                    rangeY = 1 + (int)(normalized * 63f);
                    var sys = ModContent.GetInstance<MultitoolConfigSystem>();
                    sys?.NotifySpawn(null, null, rangeY);
                },
                null,
                Color.White
            );
            heightSlider.Left.Set(5, 0f);
            heightSlider.Top.Set(0, 0.5f);
            heightSlider.Width.Set(0, 0.7f);
            heightSlider.Height.Set(8, 0f);
            MainPanel.Append(heightSlider);

            var widthSlider = new UISliderBetter(
                () => MathHelper.Clamp((rangeX - 1) / 63f, 0f, 1f),
                (value) =>
                {
                    float normalized = MathHelper.Clamp(value, 0f, 1f);
                    rangeX = 1 + (int)(normalized * 63f);
                    var sys = ModContent.GetInstance<MultitoolConfigSystem>();
                    sys?.NotifySpawn(null, rangeX, null);
                },
                null,
                Color.White
            );
            widthSlider.Left.Set(5, 0f);
            widthSlider.Top.Set(15, 0.5f);
            widthSlider.Width.Set(0, 0.7f);
            widthSlider.Height.Set(8, 0f);
            MainPanel.Append(widthSlider);

            var toggle = new UIToggleButton(
                TextureAssets.Buff[9],
                showRange
            );
            toggle.Width.Set(20, 0f);
            toggle.Height.Set(20, 0f);
            toggle.Top.Set(0, 0.5f);
            toggle.Left.Set(15, 0.7f);

            toggle.OnToggle += (isOn) =>
            {
                var sys = ModContent.GetInstance<MultitoolConfigSystem>();
                sys?.NotifySpawn(isOn, null, null);
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

            if (Main.mouseLeft && !MainPanel.IsMouseHovering)
            {
                var sys = ModContent.GetInstance<MultitoolConfigSystem>();
                sys?.HideUI();
            }
        }
    }
}