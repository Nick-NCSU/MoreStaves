using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreStaves.Projectiles
{
	// Adds the Luminite Minion as a projectile.
	public class LuminiteMinion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Luminite");

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
			projectile.width = 52;
			projectile.height = 52;

			// Makes the minion go through tiles freely
			projectile.tileCollide = false;
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

		// Allows minion to deal contact damage
		public override bool MinionContactDamage()
		{
			return true;
		}

        readonly int[] projDelay = { 0, 0, 0 };
		public override void AI()
		{
			Player player = Main.player[projectile.owner];

			#region Active check
			// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
			if (player.dead || !player.active)
			{
				player.ClearBuff(ModContent.BuffType<Buffs.LuminiteBuff>());
			}
			if (player.HasBuff(ModContent.BuffType<Buffs.LuminiteBuff>()))
			{
				projectile.timeLeft = 2;
			}
			#endregion

			#region Find target
			// Starting search distance
			float distanceFromTarget = 700f;
			Vector2 targetCenter = projectile.position;
			bool foundTarget = false;

			// If the player has targetted an npc then check its validity as a target
			if (player.HasMinionAttackTargetNPC)
			{
				NPC npc = Main.npc[player.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, projectile.Center);
				if (between < 2000f)
				{
					distanceFromTarget = between;
					targetCenter = npc.Center;
					foundTarget = true;
				}
			}
			// If no target is currently found then search all NPCs and find the closest one
			if (!foundTarget)
			{
				// This code is required either way, used for finding a target
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					if (npc.CanBeChasedBy())
					{
						float between = Vector2.Distance(npc.Center, projectile.Center);
						bool closest = Vector2.Distance(projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;
						if ((closest && inRange) || !foundTarget)
						{
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}
            #endregion

            #region Movement
            projectile.velocity = Vector2.Zero;
			if(projectile.ai[0]++ == 360)
            {
				projectile.ai[0] = 0;
            }
			float distance = 70;
			float posx = distance * (float) Math.Sin(Math.PI * projectile.ai[0] / 180);
			float posy = distance * (float) Math.Cos(Math.PI * projectile.ai[0] / 180);
			projectile.position += player.Center + new Vector2(posx, posy) - projectile.Center;
			#endregion

			#region Attack
			// Speed of projectile
			float projSpeed = 15f;
			if (projDelay[0] == 0)
			{
				if (foundTarget)
				{
					projDelay[0] = 120;
					Vector2 minionToProjectile = projectile.Center - targetCenter;
					minionToProjectile.Normalize();
					minionToProjectile *= projSpeed;
					Vector2 velocity = -minionToProjectile;
					// Spawns a projectile in direction of target
					Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<LuminiteOrbProjectile>(), 120, 1, projectile.owner);
				}
			}
			else
			{
				projDelay[0]--;
			}

			// Speed of projectile
			float projSpeed2 = 25f;
			if (projDelay[1] == 0)
			{
				if (foundTarget)
				{
					projDelay[1] = 10;
					Vector2 minionToProjectile = projectile.Center - targetCenter;
					minionToProjectile.Normalize();
					minionToProjectile *= projSpeed2;
					Vector2 velocity = -minionToProjectile;
					// Spawns a projectile in direction of target
					Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<LuminiteBulletProjectile>(), 100, 1, projectile.owner);
				}
			}
			else
			{
				projDelay[1]--;
			}

			if (projDelay[2] == 0)
			{
				if (foundTarget)
				{
					projDelay[2] = 600;
					// Spawns a projectile in direction of target
					int laser = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<LuminiteBeamProjectile>(), 250, 1, projectile.owner);
					Main.projectile[laser].ai[0] = projectile.whoAmI;
				}
			}
			else
			{
				projDelay[2]--;
			}

			#endregion

			#region Animation and visuals
			// So it will lean slightly towards the direction it's moving
			projectile.rotation = projectile.velocity.X * 0.05f;

			// Adds light around the minion
			Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 0.78f);
			#endregion

		}

		// Inflicts On Fire on contact
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(BuffID.OnFire, 300);
            base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}
