using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria;
using Terraria.UI;
using System;

namespace AutomationPlus.UI.Elements;

public class UISliderBetter : UIElement
{
    private static UISliderBetter _lockedSlider;
    private readonly Color _accentColor;
    private readonly Func<float> _getValue;
    private readonly Action<float> _setValueWithMouse;
    private readonly Action _setValueWithGamepad;

    private float value;
    private bool hovering;
    private bool wasHovering;
    private bool playedDragSound;

    private Rectangle bar;

    public static void ResetInteractionState()
    {
        _lockedSlider = null;
    }

    public UISliderBetter(
        Func<float> getValue,
        Action<float> setValueWithMouse,
        Action setValueWithGamepad,
        Color accentColor)
    {

        _getValue = getValue ?? (() => 0f);
        _setValueWithMouse = setValueWithMouse ?? (_ => { });
        _setValueWithGamepad = setValueWithGamepad;
        _accentColor = accentColor;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        spriteBatch.Draw(TextureAssets.ColorBar.Value, bar, Color.White);

        if (hovering || _lockedSlider == this)
        {
            spriteBatch.Draw(TextureAssets.ColorHighlight.Value, bar, Main.OurFavoriteColor);
            if (!wasHovering) SoundEngine.PlaySound(SoundID.MenuTick);
            wasHovering = true;
        }
        else
        {
            wasHovering = false;
        }

        // Draw slider knob
        Vector2 knobPos = new(bar.X + bar.Width * value, bar.Y + bar.Height * 0.5f);
        Vector2 knobOrigin = new(TextureAssets.ColorSlider.Value.Width * 0.5f, TextureAssets.ColorSlider.Value.Height * 0.5f);
        Vector2 knobScale = new(1f, bar.Height / (float)TextureAssets.ColorSlider.Value.Height);
        spriteBatch.Draw(TextureAssets.ColorSlider.Value, knobPos, null, _accentColor, 0f, knobOrigin, knobScale, SpriteEffects.None, 0f);
    }
    public override void Update(GameTime gameTime)
    {
        if (_lockedSlider != null && _lockedSlider.Parent == null)
        {
            _lockedSlider = null;
        }

        CalculatedStyle d = GetDimensions();
        bar = new((int)d.X, (int)d.Y, (int)d.Width, (int)d.Height);

        hovering = !IgnoresMouseInteraction && bar.Contains(new Point(Main.mouseX, Main.mouseY));

        bool dragging = PlayerInput.Triggers.Current.MouseLeft && !PlayerInput.UsingGamepad;
        if (!dragging && _lockedSlider == this) _lockedSlider = null;

        bool canDrag = !IgnoresMouseInteraction && (hovering || _lockedSlider == this);
        if (canDrag && dragging && (_lockedSlider == null || _lockedSlider == this))
        {
            _lockedSlider = this;
            float normalized = MathHelper.Clamp((Main.mouseX - bar.X) / (float)bar.Width, 0f, 1f);
            _setValueWithMouse(normalized);

            if (!playedDragSound) SoundEngine.PlaySound(SoundID.MenuTick);
            playedDragSound = true;
        }
        else
        {
            playedDragSound = false;
        }

        value = MathHelper.Clamp(_getValue(), 0f, 1f);

        if (hovering) _setValueWithGamepad?.Invoke();
    }
}