using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreStaves.Projectiles
{
    // Adds projectile shot by Copper Minion
    class CopperProjectile : ModProjectile
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
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            Main.projFrames[projectile.type] = 5;
            // Picks which tool to throw
            Random r = new Random();
            int sprite = r.Next(0, 5);
            projectile.frame = sprite;
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
            // Determines the rotation of the item
            projectile.rotation = projectile.ai[0]/5;
            // Some visuals here
            Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 0.78f);
			#endregion

		}
	}
}
