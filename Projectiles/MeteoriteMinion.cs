using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreStaves.Projectiles
{
	// Adds the Meteorite Minion as a projectile.
	public class MeteoriteMinion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Meteorite");

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

		public override void AI()
		{
			Player player = Main.player[projectile.owner];

			#region Active check
			// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
			if (player.dead || !player.active)
			{
				player.ClearBuff(ModContent.BuffType<Buffs.MeteoriteBuff>());
			}
			if (player.HasBuff(ModContent.BuffType<Buffs.MeteoriteBuff>()))
			{
				projectile.timeLeft = 2;
			}
			#endregion

			#region Movement
			float speed = 8f;
			float inertia = 20f;
			Vector2 cursorPosition = Main.MouseWorld;
			Vector2 vectorToCursor = cursorPosition - projectile.Center;
			float distanceToCursor = vectorToCursor.Length();
			if(projectile.velocity == Vector2.Zero)
            {
				// If there is a case where it's not moving at all, give it a little "poke"
				projectile.velocity -= new Vector2(0.05f, 0.01f);
            }
			else if (distanceToCursor > 80)
			{
				// Move towards cursor
				vectorToCursor.Normalize();
				vectorToCursor *= speed;
				projectile.velocity = (projectile.velocity * (inertia - 1) + vectorToCursor) / inertia;
			} 
			else if (distanceToCursor < 10)
			{
				// Move away from cursor
				vectorToCursor.Normalize();
				vectorToCursor *= speed;
				projectile.velocity = (projectile.velocity * (inertia - 1) - vectorToCursor) / inertia;
			}
			else 
            {
				// Travel in bounds around cursor
				if(vectorToCursor.X > 0 && vectorToCursor.Y > 0)
                {
					projectile.velocity += new Vector2(0, -0.01f);
				}
				else if (vectorToCursor.X < 0 && vectorToCursor.Y > 0)
				{
					projectile.velocity += new Vector2(0.01f, 0);
				}
				else if (vectorToCursor.X > 0 && vectorToCursor.Y < 0)
				{
					projectile.velocity += new Vector2(-0.01f, 0);
				}
				else if (vectorToCursor.X < 0 && vectorToCursor.Y < 0)
				{
					projectile.velocity += new Vector2(0, 0.01f);
				}
			}
			#endregion

			#region Animation and visuals
			// So it will lean slightly towards the direction it's moving
			projectile.rotation = projectile.velocity.X * 0.05f;

			// Adds light around the minion
			Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 0.78f);
			#endregion

		}

		// Inflicts On Fire when dealing contact damage
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(BuffID.OnFire, 300);
            base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}
