using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreStaves.Projectiles
{
    // Adds projectile shot by Platinum Minion
    class PlatinumProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.ignoreWater = true;
            projectile.tileCollide = true;
        }

        // Kills projectile on tile collide
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.Kill();
            return true;
        }

        // Allows projectile to deal contact damage
        public override bool MinionContactDamage()
        {
            return true;
        }

		public override void AI()
		{
            #region Movement
            float inertia = 20f;
            Vector2 direction = new Vector2(0, 0.5f);
            projectile.velocity = (projectile.velocity * (inertia - 1) + direction) / inertia;
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
