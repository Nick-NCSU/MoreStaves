using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreStaves.Buffs
{
	// Adds the buff for the Coat Rack
	public class WoodBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Coat Rack");
			Description.SetDefault("The coat rack will fight for you");

			// Don't save buff when exiting
			Main.buffNoSave[Type] = true;
			// Don't show buff time as it is (effectively) infinite
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			// If the player currently has the Coat Rack summoned then add time to buff else remove buff
			if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.WoodMinion>()] > 0)
			{
				player.buffTime[buffIndex] = 18000;
			}
			else
			{
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}
}
