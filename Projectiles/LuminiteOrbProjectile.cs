using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreStaves.Projectiles
{
    // Adds projectile shot by Luminite Minion
    class LuminiteOrbProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 58;
            projectile.height = 57;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 180;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
        }

        // Allows projectile to deal contact damage
        public override bool MinionContactDamage()
        {
            return true;
        }

		public override void AI()
		{
            #region Movement
            projectile.velocity *= 0.96f;
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
