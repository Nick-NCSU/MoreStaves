using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreStaves.Projectiles
{
	// Adds the Wood Minion as a projectile
	public class WoodMinion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Coat Rack");

			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[projectile.type] = 1;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;

			// Denotes that this projectile is a pet or minion
			Main.projPet[projectile.type] = true;
			// This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
			// Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
			ProjectileID.Sets.Homing[projectile.type] = true;
		}

		public sealed override void SetDefaults()
		{
			projectile.width = 40;
			projectile.height = 100;

			// Makes the minion not go through tiles freely
			projectile.tileCollide = true;
			projectile.friendly = true;

			// Deals minion damage
			projectile.minion = true;

			// Number of minion slots used
			projectile.minionSlots = 1f;

			// Prevents being destroyed on collision
			projectile.penetrate = -1;
		}

		// Prevents tiles being broken by minion
		public override bool? CanCutTiles()
		{
			return false;
		}

		// Disallows minion to deal contact damage
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
				player.ClearBuff(ModContent.BuffType<Buffs.WoodBuff>());
			}
			if (player.HasBuff(ModContent.BuffType<Buffs.WoodBuff>()))
			{
				projectile.timeLeft = 2;
			}
			#endregion

			#region Movement
			Vector2 down = new Vector2(0, 1);
			projectile.velocity = (projectile.velocity  + down);
			#endregion

			#region Animation and visuals
			// So it will lean slightly towards the direction it's moving
			projectile.rotation = projectile.velocity.X * 0.05f;

			// Adds light around the minion
			Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 0.78f);
			#endregion

		}
	}
}
