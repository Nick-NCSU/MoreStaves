using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreStaves.Projectiles
{
    // Adds projectile shot by Spectre Minion
    class SpectreProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 30;
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

            #region Animation and visuals
            if (projectile.ai[0]++ >= 360)
            {
                projectile.ai[0] = 0;
            }
            projectile.rotation = projectile.ai[0]/5;
            
            // Adds light around the minion
            Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 0.78f);
			#endregion

		}
	}
}
