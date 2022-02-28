using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreStaves.Projectiles
{
    // Adds projectile shot by Ascendant Minion
    class AscendantTargetedMarkProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.Homing[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.minion = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 1000;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
        }

        // Disallows projectile to deal contact damage
        public override bool MinionContactDamage()
        {
            return false;
        }

		public override void AI()
		{

            #region Animation and visuals
			// Adds light around the minion
			Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 0.78f);
			#endregion

		}
	}
}
