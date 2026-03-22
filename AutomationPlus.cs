using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Terraria.Graphics.Shaders;

namespace AutomationPlus
{
	public class AutomationPlus : Mod
	{
		public static void LoadMiscShader(string name, string pass)
		{
			var shaderAsset = ModContent.Request<Effect>("AutomationPlus/Effects/" + name, ReLogic.Content.AssetRequestMode.ImmediateLoad);
			GameShaders.Misc["AutomationPlus:" + name] = new MiscShaderData(shaderAsset, pass);
		}
		public override void Load()
		{
			base.Load();
		}
	}
}
