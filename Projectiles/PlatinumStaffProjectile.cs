using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreStaves.Projectiles
{
    // Adds the Platinum Staff Projectile which is displayed when using the Staff
    class PlatinumStaffProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // Sets the total frames of this projectile to 30
            Main.projFrames[projectile.type] = 30;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 36;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            // Allows the projectile to start on a random frame
            projectile.frame = new Random().Next(0, 30);
        }

        // Shares texture with Chlorophyte Staff
        public override string Texture => "MoreStaves/Items/PlatinumStaff";

        public override void AI()
		{
            #region Animation and visuals
            Player player = Main.player[projectile.owner];
            UpdatePlayerVisuals(player, player.RotatedRelativePoint(player.MountedCenter, true));
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

        // Moves the projectile to the direction of the player
        private void UpdatePlayerVisuals(Player player, Vector2 playerHandPos)
        {
            if (player.direction == 1)
            {
                projectile.Center = playerHandPos - projectile.Size + new Vector2(10, -13);
                projectile.rotation = -0.4f;
            }
            else
            {
                projectile.Center = playerHandPos - projectile.Size + new Vector2(-9, -13);
                projectile.rotation = 0.4f;
            }
            projectile.spriteDirection = player.direction;

            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            player.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
        }
	}
}
