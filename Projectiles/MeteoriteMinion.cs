using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DubNation.Projectiles
{
	public class MeteoriteMinion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Meteorite");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[projectile.type] = 1;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;

			// These below are needed for a minion
			// Denotes that this projectile is a pet or minion
			Main.projPet[projectile.type] = true;
			// This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
			// Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
			ProjectileID.Sets.Homing[projectile.type] = true;
		}

		public sealed override void SetDefaults()
		{
			projectile.width = 52;
			projectile.height = 52;
			// Makes the minion go through tiles freely
			projectile.tileCollide = false;

			// These below are needed for a minion weapon
			// Only controls if it deals damage to enemies on contact (more on that later)
			projectile.friendly = true;
			// Only determines the damage type
			projectile.minion = true;
			// Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			projectile.minionSlots = 1f;
			// Needed so the minion doesn't despawn on collision with enemies or tiles
			projectile.penetrate = -1;
		}

		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles()
		{
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
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
			float speed = 6f;
			float inertia = 20f;
			Vector2 cursorPosition = Main.MouseWorld;
			Vector2 vectorToCursor = cursorPosition - projectile.Center;
			float distanceToCursor = vectorToCursor.Length();
			if(projectile.velocity == Vector2.Zero)
            {
				projectile.velocity -= new Vector2(0.05f, 0.01f);
            }
			else if (distanceToCursor > 80)
			{
				vectorToCursor.Normalize();
				vectorToCursor *= speed;
				projectile.velocity = (projectile.velocity * (inertia - 1) + vectorToCursor) / inertia;
			} 
			else if (distanceToCursor < 10)
			{
				vectorToCursor.Normalize();
				vectorToCursor *= speed;
				projectile.velocity = (projectile.velocity * (inertia - 1) - vectorToCursor) / inertia;
			}
			else 
            {
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

			// This is a simple "loop through all frames from top to bottom" animation
			int frameSpeed = 5;
			projectile.frameCounter++;
			if (projectile.frameCounter >= frameSpeed)
			{
				projectile.frameCounter = 0;
				projectile.frame++;
				if (projectile.frame >= Main.projFrames[projectile.type])
				{
					projectile.frame = 0;
				}
			}

			// Some visuals here
			Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 0.78f);
			#endregion

		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(BuffID.OnFire, 300);
            base.OnHitNPC(target, damage, knockback, crit);
        }
    }
}
