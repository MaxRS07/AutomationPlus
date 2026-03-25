
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.UI.Elements;

namespace AutomationPlus.UI.Elements
{
    public class UIToggleButton : UIImageButton
    {
        private bool _isOn;

        private Asset<Texture2D> _icon;

        public event Action<bool> OnToggle;

        public UIToggleButton(Asset<Texture2D> icon, bool initialState = false) : base(icon)
        {
            _isOn = initialState;
            _icon = icon;
            OnLeftClick += (_, _) => { _isOn = !_isOn; OnToggle?.Invoke(_isOn); };
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            var texture = _icon.Value;
            var position = GetInnerDimensions().Position();
            var dest = new Rectangle((int)position.X, (int)position.Y, (int)Width.Pixels, (int)Height.Pixels);
            if (IsMouseHovering)
            {
                dest.Inflate(2, 2);
            }
            var color = _isOn ? Color.White : Color.Gray;

            Main.spriteBatch.Draw(texture, dest, color);
        }
    }
}