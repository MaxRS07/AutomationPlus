using Terraria.UI;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace AutomationPlus.UI.Elements
{
    public class UIArrowButton : UIImageButton
    {
        private static Asset<Texture2D> ArrowTexture => ModContent.Request<Texture2D>("AutomationPlus/Assets/Arrow", ReLogic.Content.AssetRequestMode.ImmediateLoad);
        public enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }

        public Direction ButtonDirection { get; private set; }

        public UIArrowButton(Direction direction) : base(ArrowTexture)
        {
            ButtonDirection = direction;

            Width.Set(12, 0f);
            Height.Set(12, 0f);
        }

        public override void Update(GameTime gameTime)
        {
            OnLeftClick += (evt, element) =>
            {
                OnChangeDirection?.Invoke(ButtonDirection);
            };
            base.Update(gameTime);
        }

        public event Action<Direction> OnChangeDirection;

        public override void Draw(SpriteBatch spriteBatch)
        {
            var rotation = ButtonDirection switch
            {
                Direction.Right => 0f,
                Direction.Down => MathHelper.PiOver2,
                Direction.Left => MathHelper.Pi,
                _ => -MathHelper.PiOver2,
            };
            var texture = ArrowTexture.Value;

            spriteBatch.Draw(
                ArrowTexture.Value,
                GetDimensions().Center(),
                null,
                Color.White * (IsMouseHovering ? 1f : 0.4f),
                rotation,
                texture.Size() / 2f,
                0.5f,
                SpriteEffects.None,
                0f
            );
        }
    }
}