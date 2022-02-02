using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreStaves.Projectiles
{
    class IronProjectile : ModProjectile
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
            Random r = new Random();
            int sprite = r.Next(0, 5);
            projectile.frame = sprite;
        }

        public override bool MinionContactDamage()
        {
            return true;
        }

		public override void AI()
		{

            #region Animation and visuals
            // So it will lean slightly towards the direction it's moving
            if (projectile.ai[0]++ >= 360)
            {
                projectile.ai[0] = 0;
            }
            projectile.rotation = projectile.ai[0] / 5;

            // Some visuals here
            Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 0.78f);
			#endregion

		}
	}
}
