using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DubNation.Projectiles
{
    class ChlorophyteProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 240;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

		public override void AI()
		{
			#region Find target
			// Starting search distance
			float distanceFromTarget = 700f;
			Vector2 targetCenter = projectile.position;
			bool foundTarget = false;
			Player player = Main.player[projectile.owner];
			// This code is required if your minion weapon has the targeting feature
			if (player.HasMinionAttackTargetNPC)
			{
				NPC npc = Main.npc[player.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, projectile.Center);
				// Reasonable distance away so it doesn't target across multiple screens
				if (between < 2000f)
				{
					distanceFromTarget = between;
					targetCenter = npc.Center;
					projectile.hide = false;
					foundTarget = true;
				}
			}
			if (!foundTarget)
			{
				projectile.hide = true;
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
							projectile.hide = false;
							foundTarget = true;
						}
					}
				}
			}
			#endregion

			#region Movement
			// Default movement parameters (here for attacking)
			float speed = 15f * projectile.timeLeft / 240;
			float inertia = 20f;
			if (foundTarget)
			{
				// The immediate range around the target (so it doesn't latch onto it when close)
				if (distanceFromTarget > 20)
				{
					Vector2 direction = targetCenter - projectile.Center;
					direction.Normalize();
					direction *= speed;
					projectile.velocity = (projectile.velocity * (inertia - 1) + direction) / inertia;
				}
			}
			#endregion

			#region Animation and visuals
			if (foundTarget)
			{
				projectile.rotation = projectile.DirectionTo(targetCenter).ToRotation() + 90;
			}
			else
			{
				projectile.rotation = projectile.velocity.ToRotation() + 90;
			}
			// Some visuals here
			Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 0.78f);
			#endregion

		}
	}
}
