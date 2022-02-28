using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreStaves.Projectiles
{
	// Adds the Ascendant Minion as a projectile
	public class AscendantMinion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ascendant");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[projectile.type] = 2;
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
			projectile.width = 22;
			projectile.height = 42;

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

		// Disallows minion to deal contact damage
		public override bool MinionContactDamage()
		{
			return false;
		}

        // The charge of each attack
        readonly int[] charge = { 0, 0, 0, 0, 0 };
		// Projectile id of each mark
		readonly int[] marks = { -1, -1, -1, -1, -1 };
		// NPC id of each target
		readonly int[] targets = { -1, -1, -1, -1, -1 };

		// Cluster phase
		int cluster = 0;
		// Cluster position
		Vector2 clusterPos;
		// Cluster attack delay
		int clusterDelay = 0;
		// Movement direction
		Boolean down = false;
		public override void AI()
		{
			Player player = Main.player[projectile.owner];

			#region Active check
			// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
			if (player.dead || !player.active)
			{
				player.ClearBuff(ModContent.BuffType<Buffs.AscendantBuff>());
			}
			if (player.HasBuff(ModContent.BuffType<Buffs.AscendantBuff>()))
			{
				projectile.timeLeft = 2;
			}
			#endregion

			#region Find target
			// Starting search distance
			float distanceFromTarget = 900f;

			// If the player is targetting an NPC then set it to the 5th target.
			// Otherwise mark no 5th target
			if (player.HasMinionAttackTargetNPC)
			{
				NPC npc = Main.npc[player.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, projectile.Center);
				// Reasonable distance away so it doesn't target across multiple screens
				if (between < distanceFromTarget)
				{
					targets[4] = player.MinionAttackTargetNPC;
				}
				else
				{
					targets[4] = -1;
				}
			}
			else
			{
				targets[4] = -1;
			}

			// For each target check if it is within range
			// Else remove it as a target
			for (int i = 0; i < 5; i++)
			{
				if (targets[i] == -1)
				{
					if (marks[i] != -1)
					{
						Main.projectile[marks[i]].Kill();
						marks[i] = -1;
					}
					charge[i] = 0;
					continue;
				}
				NPC npc = Main.npc[targets[i]];
				if (npc.CanBeChasedBy())
				{
					if (Vector2.Distance(npc.Center, projectile.Center) >= distanceFromTarget)
					{
						if (marks[i] != -1)
						{
							Main.projectile[marks[i]].Kill();
							marks[i] = -1;
						}
						targets[i] = -1;
						charge[i] = 0;
					}
				}
				else
				{
					if (marks[i] != -1)
					{
						Main.projectile[marks[i]].Kill();
						marks[i] = -1;
					}
					targets[i] = -1;
					charge[i] = 0;
				}
			}

			// Find other targets if possible
			for (int c = 0; c < 4; c++)
			{
				for (int i = 0; targets[c] == -1 && i < Main.maxNPCs; i++)
				{
					if (!CheckTargets(i))
					{
						continue;
					}
					NPC npc = Main.npc[i];
					if (npc.CanBeChasedBy())
					{
						float between = Vector2.Distance(npc.Center, projectile.Center);
						bool inRange = between < distanceFromTarget;
						if (inRange)
						{
							targets[c] = i;
						}
					}
				}
			}
			#endregion

			#region Movement
			if (down)
			{
				if (projectile.ai[0]-- == 0)
				{
					down = false;
				}
			}
			else
			{
				if (projectile.ai[0]++ >= 500)
				{
					down = true;
				}
			}
			projectile.position += player.Center - new Vector2(0, (projectile.ai[0] / 20) + 60f) - projectile.Center;
			#endregion

			#region Attack
			// Attack each found target
			for (int i = 0; i < 5; i++)
			{
				if (targets[i] == -1)
				{
					continue;
				}
				if (charge[i] > 50)
				{
					Vector2 position = Main.npc[targets[i]].Center;
					Main.PlayTrackedSound(SoundID.DD2_ExplosiveTrapExplode);
					Projectile.NewProjectile(position, new Vector2(0, 0), ProjectileID.DD2ExplosiveTrapT3Explosion, 3000, 0, projectile.owner);
					if (i == 4)
					{
						clusterPos = position;
						clusterDelay = 20;
						cluster = 1;
					}
					Main.projectile[marks[i]].Kill();
					targets[i] = -1;
					marks[i] = -1;
					charge[i] = 0;
				}
				else
				{
					if (marks[i] == -1)
					{
						marks[i] = Projectile.NewProjectile(projectile.Center, new Vector2(0, 0), i == 4 ? ModContent.ProjectileType<AscendantTargetedMarkProjectile>() : ModContent.ProjectileType<AscendantMarkProjectile>(), 0, 1, projectile.owner);
					}
					Main.projectile[marks[i]].scale *= 1.02f;
					Main.projectile[marks[i]].position = Main.npc[targets[i]].Center - Main.projectile[marks[i]].Size / 2;
					charge[i]++;
				}
			}
			if (clusterDelay == 0)
			{
				clusterDelay = 20;
				if (cluster == 1)
				{
					Main.PlayTrackedSound(SoundID.DD2_ExplosiveTrapExplode);
					Projectile.NewProjectile(clusterPos - new Vector2(-100f, 0), new Vector2(0, 0), ProjectileID.DD2ExplosiveTrapT3Explosion, 2000, 0, projectile.owner);
					Projectile.NewProjectile(clusterPos - new Vector2(100f, 0), new Vector2(0, 0), ProjectileID.DD2ExplosiveTrapT3Explosion, 2000, 0, projectile.owner);
					Projectile.NewProjectile(clusterPos - new Vector2(0, -100f), new Vector2(0, 0), ProjectileID.DD2ExplosiveTrapT3Explosion, 2000, 0, projectile.owner);
					Projectile.NewProjectile(clusterPos - new Vector2(0, 100f), new Vector2(0, 0), ProjectileID.DD2ExplosiveTrapT3Explosion, 2000, 0, projectile.owner);
					cluster = 2;
				}
				else if (cluster == 2)
				{
					Vector2[] centers =
					{
						clusterPos -  new Vector2(-100f, 0),
						clusterPos -  new Vector2(100f, 0),
						clusterPos -  new Vector2(0, -100f),
						clusterPos -  new Vector2(0, 100f)
					};
					foreach (Vector2 center in centers)
					{
						Main.PlayTrackedSound(SoundID.DD2_ExplosiveTrapExplode);
						Projectile.NewProjectile(center - new Vector2(-40f, 0), new Vector2(0, 0), ProjectileID.DD2ExplosiveTrapT3Explosion, 1000, 0, projectile.owner);
						Projectile.NewProjectile(center - new Vector2(40f, 0), new Vector2(0, 0), ProjectileID.DD2ExplosiveTrapT3Explosion, 1000, 0, projectile.owner);
						Projectile.NewProjectile(center - new Vector2(0, -40f), new Vector2(0, 0), ProjectileID.DD2ExplosiveTrapT3Explosion, 1000, 0, projectile.owner);
						Projectile.NewProjectile(center - new Vector2(0, 40f), new Vector2(0, 0), ProjectileID.DD2ExplosiveTrapT3Explosion, 1000, 0, projectile.owner);
					}
					cluster = 0;
				}
			}
			else
			{
				clusterDelay--;
			}
			#endregion

			#region Animation and visuals

			// This is a simple "loop through all frames from top to bottom" animation
			if (InCombat())
			{
				projectile.frame = 0;
			}
			else
            {
				projectile.frame = 1;
            }
			#endregion

		}

		private Boolean CheckTargets(int n)
        {
			for (int i = 0; i < 5; i++)
			{
				if (targets[i] == n)
				{
					return false;
				}
			}
			return true;
		}

		private Boolean InCombat()
        {
			for (int i = 0; i < 5; i++)
			{
				if (targets[i] != -1)
				{
					return true;
				}
			}
			return false;
		}

		// Inflicts On Fire on contact
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(BuffID.OnFire, 300);
            base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}
