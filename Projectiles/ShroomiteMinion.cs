using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreStaves.Projectiles
{
	// Adds the Shroomite Minion as a projectile.
	public class ShroomiteMinion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shroomite");

			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[projectile.type] = 18;
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
			projectile.width = 50;
			projectile.height = 48;

			// Makes the minion go through tiles freely
			projectile.tileCollide = true;
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

		// Disallows minion to deal contact damage
		public override bool MinionContactDamage()
		{
			return false;
		}

		int delay = 0;
		int target = -1;
		// Adapted from Terraria Source Code AI Style 26
		public override void AI()
		{
			projectile.tileCollide = true;
			float projSpeed2 = 12f;
			if (delay == 0)
			{
				if (target != -1)
				{
					delay = 30;
					NPC targetNPC = Main.npc[target];
					Vector2 minionToProjectile = projectile.Center - targetNPC.Center;
					minionToProjectile.Normalize();
					minionToProjectile *= projSpeed2;
					Vector2 velocity = -minionToProjectile;
					Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<ShroomiteProjectile>(), 60, 8, projectile.owner);
				}
			}
			else
			{
				delay--;
			}
			if (Main.player[projectile.owner].dead || !Main.player[projectile.owner].active)
			{
				Main.player[projectile.owner].ClearBuff(ModContent.BuffType<Buffs.ShroomiteBuff>());
				return;
			}
			if (Main.player[projectile.owner].HasBuff(ModContent.BuffType<Buffs.ShroomiteBuff>()))
			{
				projectile.timeLeft = 2;
			}
			bool flag = false;
			bool flag2 = false;
			bool flag4 = false;
			if (projectile.lavaWet)
			{
				projectile.ai[0] = 1f;
				projectile.ai[1] = 0f;
			}
            int num = 10;
			int num2 = 40 * (projectile.minionPos + 1) * Main.player[projectile.owner].direction;
			if (Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) < projectile.position.X + (float)(projectile.width / 2) - (float)num + (float)num2)
			{
				flag = true;
			}
			else if (Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) > projectile.position.X + (float)(projectile.width / 2) + (float)num + (float)num2)
			{
				flag2 = true;
			}
			
			if (projectile.ai[1] == 0f)
			{
				int num73 = 500;
				num73 += 40 * projectile.minionPos;
				if (projectile.localAI[0] > 0f)
				{
					num73 += 500;
				}
				if (Main.player[projectile.owner].rocketDelay2 > 0)
				{
					projectile.ai[0] = 1f;
				}
				Vector2 vector7 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num74 = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector7.X;
				float num75 = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - vector7.Y;
				float num76 = (float)Math.Sqrt(num74 * num74 + num75 * num75);
				if (num76 > 2000f)
				{
					projectile.position.X = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - (float)(projectile.width / 2);
					projectile.position.Y = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - (float)(projectile.height / 2);
				}
				else if (num76 > (float)num73 || (Math.Abs(num75) > 300f &&  !(projectile.localAI[0] > 0f)))
				{
					if (num75 > 0f && projectile.velocity.Y < 0f)
					{
						projectile.velocity.Y = 0f;
					}
					if (num75 < 0f && projectile.velocity.Y > 0f)
					{
						projectile.velocity.Y = 0f;
					}
					projectile.ai[0] = 1f;
				}
			}
			if (projectile.ai[0] != 0f)
			{
                int num78 = 100;
				projectile.tileCollide = false;
				Vector2 vector8 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num79 = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector8.X;
				num79 -= (float)(40 * Main.player[projectile.owner].direction);
				float num80 = 800f;
				bool flag7 = false;
				int num81 = -1;
				for (int num82 = 0; num82 < 200; num82++)
				{
					if (!Main.npc[num82].CanBeChasedBy(projectile))
					{
						continue;
					}
					float num83 = Main.npc[num82].position.X + (float)(Main.npc[num82].width / 2);
					float num84 = Main.npc[num82].position.Y + (float)(Main.npc[num82].height / 2);
					if (Math.Abs(Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - num83) + Math.Abs(Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - num84) < num80)
					{
						if (Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[num82].position, Main.npc[num82].width, Main.npc[num82].height))
						{
							num81 = num82;
						}
						flag7 = true;
						break;
					}
				}
				if (!flag7)
				{
					num79 -= (float)(40 * projectile.minionPos * Main.player[projectile.owner].direction);
				}
				if (flag7 && num81 >= 0)
				{
					projectile.ai[0] = 0f;
				}
				float num85 = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - vector8.Y;
				float num86 = (float)Math.Sqrt(num79 * num79 + num85 * num85);
                float num77 = 0.4f;
                float num88 = 12f;
                if (num88 < Math.Abs(Main.player[projectile.owner].velocity.X) + Math.Abs(Main.player[projectile.owner].velocity.Y))
				{
					num88 = Math.Abs(Main.player[projectile.owner].velocity.X) + Math.Abs(Main.player[projectile.owner].velocity.Y);
				}
				if (num86 < (float)num78 && Main.player[projectile.owner].velocity.Y == 0f && projectile.position.Y + (float)projectile.height <= Main.player[projectile.owner].position.Y + (float)Main.player[projectile.owner].height && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
				{
					projectile.ai[0] = 0f;
					if (projectile.velocity.Y < -6f)
					{
						projectile.velocity.Y = -6f;
					}
				}
				if (num86 < 60f)
				{
					num79 = projectile.velocity.X;
					num85 = projectile.velocity.Y;
				}
				else
				{
					num86 = num88 / num86;
					num79 *= num86;
					num85 *= num86;
				}
				if (projectile.velocity.X < num79)
				{
					projectile.velocity.X += num77;
					if (projectile.velocity.X < 0f)
					{
						projectile.velocity.X += num77 * 1.5f;
					}
				}
				if (projectile.velocity.X > num79)
				{
					projectile.velocity.X -= num77;
					if (projectile.velocity.X > 0f)
					{
						projectile.velocity.X -= num77 * 1.5f;
					}
				}
				if (projectile.velocity.Y < num85)
				{
					projectile.velocity.Y += num77;
					if (projectile.velocity.Y < 0f)
					{
						projectile.velocity.Y += num77 * 1.5f;
					}
				}
				if (projectile.velocity.Y > num85)
				{
					projectile.velocity.Y -= num77;
					if (projectile.velocity.Y > 0f)
					{
						projectile.velocity.Y -= num77 * 1.5f;
					}
				}
				if (projectile.frame < 12)
				{
					projectile.frame = Main.rand.Next(12, 18);
					projectile.frameCounter = 0;
				}
				if ((double)projectile.velocity.X > 0.5)
				{
					projectile.spriteDirection = -1;
				}
				else if ((double)projectile.velocity.X < -0.5)
				{
					projectile.spriteDirection = 1;
				}
				if (projectile.spriteDirection == -1)
				{
					projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X);
				}
				else
				{
					projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 3.14f;
				}
			}
			else
			{
				float num106 = 40 * projectile.minionPos;
				int num107 = 30;
				int num108 = 60;
				projectile.localAI[0] -= 1f;
				if (projectile.localAI[0] < 0f)
				{
					projectile.localAI[0] = 0f;
				}
				if (projectile.ai[1] > 0f)
				{
					projectile.ai[1] -= 1f;
				}
				else
				{
					float num109 = projectile.position.X;
					float num110 = projectile.position.Y;
					float num111 = 100000f;
					float num112 = num111;
					int num113 = -1;
					NPC ownerMinionAttackTargetNPC = projectile.OwnerMinionAttackTargetNPC;
					if (ownerMinionAttackTargetNPC != null && ownerMinionAttackTargetNPC.CanBeChasedBy(projectile))
					{
						float num114 = ownerMinionAttackTargetNPC.position.X + (float)(ownerMinionAttackTargetNPC.width / 2);
						float num115 = ownerMinionAttackTargetNPC.position.Y + (float)(ownerMinionAttackTargetNPC.height / 2);
						float num116 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num114) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num115);
						if (num116 < num111)
						{
							if (num113 == -1 && num116 <= num112)
							{
								num112 = num116;
								num109 = num114;
								num110 = num115;
							}
							if (Collision.CanHit(projectile.position, projectile.width, projectile.height, ownerMinionAttackTargetNPC.position, ownerMinionAttackTargetNPC.width, ownerMinionAttackTargetNPC.height))
							{
								num111 = num116;
								num109 = num114;
								num110 = num115;
								num113 = ownerMinionAttackTargetNPC.whoAmI;
							}
						}
					}
					if (num113 == -1)
					{
						for (int num117 = 0; num117 < 200; num117++)
						{
							if (!Main.npc[num117].CanBeChasedBy(projectile))
							{
								continue;
							}
							float num118 = Main.npc[num117].position.X + (float)(Main.npc[num117].width / 2);
							float num119 = Main.npc[num117].position.Y + (float)(Main.npc[num117].height / 2);
							float num120 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num118) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num119);
							if (num120 < num111)
							{
								if (num113 == -1 && num120 <= num112)
								{
									num112 = num120;
									num109 = num118;
									num110 = num119;
								}
								if (Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[num117].position, Main.npc[num117].width, Main.npc[num117].height))
								{
									num111 = num120;
									num109 = num118;
									num110 = num119;
									num113 = num117;
									target = num117;
								}
							}
						}
					}
					if (num113 == -1)
                    {
						target = -1;
                    }
					if (num113 == -1 && num112 < num111)
					{
						num111 = num112;
					}
					float num121 = 400f;
					if ((double)projectile.position.Y > Main.worldSurface * 16.0)
					{
						num121 = 200f;
					}
					if (num111 < num121 + num106 && num113 == -1)
					{
						float num122 = num109 - (projectile.position.X + (float)(projectile.width / 2));
						if (num122 < -5f)
						{
							flag = true;
							flag2 = false;
						}
						else if (num122 > 5f)
						{
							flag2 = true;
							flag = false;
						}
					}
					else if (num113 >= 0 && num111 < 800f + num106)
					{
						projectile.localAI[0] = num108;
						float num123 = num109 - (projectile.position.X + (float)(projectile.width / 2));
						if (num123 > 450f || num123 < -450f)
						{
							if (num123 < -50f)
							{
								flag = true;
								flag2 = false;
							}
							else if (num123 > 50f)
							{
								flag2 = true;
								flag = false;
							}
						}
						else if (projectile.owner == Main.myPlayer)
						{
							projectile.ai[1] = num107;
							Vector2 vector11 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)(projectile.height / 2) - 8f);
							float num124 = num109 - vector11.X + (float)Main.rand.Next(-20, 21);
							float num125 = Math.Abs(num124) * 0.1f;
							num125 = num125 * (float)Main.rand.Next(0, 100) * 0.001f;
							float num126 = num110 - vector11.Y + (float)Main.rand.Next(-20, 21) - num125;
							float num127 = (float)Math.Sqrt(num124 * num124 + num126 * num126);
							num127 = 18f / num127;
							num124 *= num127;
                            if (num124 < 0f)
							{
								projectile.direction = -1;
							}
							if (num124 > 0f)
							{
								projectile.direction = 1;
							}
							projectile.netUpdate = true;
						}
					}
				}
				bool flag9 = false;
				if (projectile.ai[1] != 0f)
				{
					flag = false;
					flag2 = false;
				}
				else if (projectile.localAI[0] == 0f)
				{
					projectile.direction = Main.player[projectile.owner].direction;
				}
				if (!flag9)
				{
					projectile.rotation = 0f;
				}
				float num154 = 0.08f;
				float num155 = 6.5f;
				if (flag)
				{
					if ((double)projectile.velocity.X > -3.5)
					{
						projectile.velocity.X -= num154;
					}
					else
					{
						projectile.velocity.X -= num154 * 0.25f;
					}
				}
				else if (flag2)
				{
					if ((double)projectile.velocity.X < 3.5)
					{
						projectile.velocity.X += num154;
					}
					else
					{
						projectile.velocity.X += num154 * 0.25f;
					}
				}
				else
				{
					projectile.velocity.X *= 0.9f;
					if (projectile.velocity.X >= 0f - num154 && projectile.velocity.X <= num154)
					{
						projectile.velocity.X = 0f;
					}
				}
				if (flag || flag2)
				{
					int num156 = (int)(projectile.position.X + (float)(projectile.width / 2)) / 16;
					int j2 = (int)(projectile.position.Y + (float)(projectile.height / 2)) / 16;
					if (flag)
					{
						num156--;
					}
					if (flag2)
					{
						num156++;
					}
					num156 += (int)projectile.velocity.X;
					if (WorldGen.SolidTile(num156, j2))
					{
						flag4 = true;
					}
				}
				Collision.StepUp(ref projectile.position, ref projectile.velocity, projectile.width, projectile.height, ref projectile.stepSpeed, ref projectile.gfxOffY);
				if (projectile.velocity.Y == 0f)
				{
					if ((projectile.velocity.X < 0f || projectile.velocity.X > 0f))
					{
						int num157 = (int)(projectile.position.X + (float)(projectile.width / 2)) / 16;
						int j3 = (int)(projectile.position.Y + (float)(projectile.height / 2)) / 16 + 1;
						if (flag)
						{
							num157--;
						}
						if (flag2)
						{
							num157++;
						}
						WorldGen.SolidTile(num157, j3);
					}
					if (flag4)
					{
						int num158 = (int)(projectile.position.X + (float)(projectile.width / 2)) / 16;
						int num159 = (int)(projectile.position.Y + (float)projectile.height) / 16;
						if (WorldGen.SolidTileAllowBottomSlope(num158, num159) || Main.tile[num158, num159].halfBrick() || Main.tile[num158, num159].slope() > 0)
						{
							try
							{
								num158 = (int)(projectile.position.X + (float)(projectile.width / 2)) / 16;
								num159 = (int)(projectile.position.Y + (float)(projectile.height / 2)) / 16;
								if (flag)
								{
									num158--;
								}
								if (flag2)
								{
									num158++;
								}
								num158 += (int)projectile.velocity.X;
								if (!WorldGen.SolidTile(num158, num159 - 1) && !WorldGen.SolidTile(num158, num159 - 2))
								{
									projectile.velocity.Y = -5.1f;
								}
								else if (!WorldGen.SolidTile(num158, num159 - 2))
								{
									projectile.velocity.Y = -7.1f;
								}
								else if (WorldGen.SolidTile(num158, num159 - 5))
								{
									projectile.velocity.Y = -11.1f;
								}
								else if (WorldGen.SolidTile(num158, num159 - 4))
								{
									projectile.velocity.Y = -10.1f;
								}
								else
								{
									projectile.velocity.Y = -9.1f;
								}
							}
							catch
							{
								projectile.velocity.Y = -9.1f;
							}
						}
					}
				}
				if (projectile.velocity.X > num155)
				{
					projectile.velocity.X = num155;
				}
				if (projectile.velocity.X < 0f - num155)
				{
					projectile.velocity.X = 0f - num155;
				}
				if (projectile.velocity.X < 0f)
				{
					projectile.direction = -1;
				}
				if (projectile.velocity.X > 0f)
				{
					projectile.direction = 1;
				}
				if (projectile.velocity.X > num154 && flag2)
				{
					projectile.direction = 1;
				}
				if (projectile.velocity.X < 0f - num154 && flag)
				{
					projectile.direction = -1;
				}
				if (projectile.direction == -1)
				{
					projectile.spriteDirection = 1;
				}
				if (projectile.direction == 1)
				{
					projectile.spriteDirection = -1;
				}
				bool flag11 = projectile.position.X - projectile.oldPosition.X == 0f;
				if (projectile.ai[1] > 0f)
				{
					if (projectile.localAI[1] == 0f)
					{
						projectile.localAI[1] = 1f;
						projectile.frame = 1;
					}
					if (projectile.frame != 0)
					{
						projectile.frameCounter++;
						if (projectile.frameCounter > 4)
						{
							projectile.frame++;
							projectile.frameCounter = 0;
						}
						if (projectile.frame >= 4)
						{
							projectile.frame = 0;
						}
					}
				}
				else if (projectile.velocity.Y == 0f)
				{
					projectile.localAI[1] = 0f;
					if (flag11)
					{
						projectile.frame = 0;
						projectile.frameCounter = 0;
					}
					else if ((double)projectile.velocity.X < -0.8 || (double)projectile.velocity.X > 0.8)
					{
						projectile.frameCounter += (int)Math.Abs(projectile.velocity.X);
						projectile.frameCounter++;
						if (projectile.frameCounter > 6)
						{
							projectile.frame++;
							projectile.frameCounter = 0;
						}
						if (projectile.frame < 5)
						{
							projectile.frame = 5;
						}
						if (projectile.frame >= 11)
						{
							projectile.frame = 5;
						}
					}
					else
					{
						projectile.frame = 0;
						projectile.frameCounter = 0;
					}
				}
				else if (projectile.velocity.Y < 0f)
				{
					projectile.frameCounter = 0;
					projectile.frame = 4;
				}
				else if (projectile.velocity.Y > 0f)
				{
					projectile.frameCounter = 0;
					projectile.frame = 4;
				}
				projectile.velocity.Y += 0.4f;
				if (projectile.velocity.Y > 10f)
				{
					projectile.velocity.Y = 10f;
				}
				_ = projectile.velocity;
			}
		}
    }
}
