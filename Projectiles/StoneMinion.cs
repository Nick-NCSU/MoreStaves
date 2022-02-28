using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreStaves.Projectiles
{
	// Adds the Stone Minion as a projectile.
	public class StoneMinion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Boulder");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[projectile.type] = 1;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;

			// Denotes that this projectile is a pet or minion
			Main.projPet[projectile.type] = true;
			// Ensures minion can properly spawn when summoned and is replaced when other minions are summoned
			ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
			// Damage reduction related to homing attacks
			ProjectileID.Sets.Homing[projectile.type] = true;
		}

		public sealed override void SetDefaults()
		{
			projectile.width = 96;
			projectile.height = 96;

			// Makes the minion go through tiles freely
			projectile.tileCollide = true;
			projectile.friendly = true;

			// Deals minion damage
			projectile.minion = true;

			// Number of minion slots used
			projectile.minionSlots = 1f;
			
			// Destroys after 5 hits
			projectile.penetrate = 5;
			
			// Despawns after 2 seconds
			projectile.timeLeft = 120;
		}

		// Prevents tiles being broken by minion
		public override bool? CanCutTiles()
		{
			return false;
		}

		// Allows minion to deal contact damage
		public override bool MinionContactDamage()
		{
			return true;
		}

		public override void AI()
		{
			Player player = Main.player[projectile.owner];

			#region Active check
			// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
			if (player.dead || !player.active)
			{
				player.ClearBuff(ModContent.BuffType<Buffs.StoneBuff>());
			}
			if (!player.HasBuff(ModContent.BuffType<Buffs.StoneBuff>()))
			{
				projectile.Kill();
			}
			#endregion

			#region Movement
			Vector2 down = new Vector2(0, 1);
			projectile.velocity = (projectile.velocity  + down);
			#endregion

			#region Animation and visuals
			// Adds light around the minion
			Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 0.78f);
			#endregion

		}
	}
}
