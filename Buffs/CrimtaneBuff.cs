using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreStaves.Buffs
{
	// Adds the buff for the Crimtane Minion
	public class CrimtaneBuff : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Crimtane Minion");
			Description.SetDefault("The crimtane minion will fight for you");

			// Don't save buff when exiting
			Main.buffNoSave[Type] = true;
			// Don't show buff time as it is (effectively) infinite
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			// If the player currently has the Crimtane Minion summoned then add time to buff else remove buff
			if (player.ownedProjectileCounts[ModContent.ProjectileType<Projectiles.CrimtaneMinion>()] > 0)
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
