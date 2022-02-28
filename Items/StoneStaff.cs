using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace MoreStaves.Items
{
	// Adds the stone staff to the game
	class StoneStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stone Staff");
			Tooltip.SetDefault("Summons a boulder to fight for you.\nBreaks after 5 attacks. Crumbles after 2 seconds.");

			// Sets the item to use 30 frames with with 5 ticks per frame
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 30));

			// Allows targeting across screen
			ItemID.Sets.GamepadWholeScreenUseRange[item.type] = true;
			ItemID.Sets.LockOnIgnoresCollision[item.type] = true;
		}

		public override void SetDefaults()
		{
			item.damage = 18;
			item.width = 20;
			item.height = 20;
			item.useTime = 120;
			item.useAnimation = 120;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = 10;
			item.rare = ItemRarityID.White;
			item.UseSound = SoundID.Item1;
			item.mana = 10;
			item.noMelee = true;
			item.summon = true;
			item.knockBack = 5f;
			// Spawns the Stone Buff
			item.buffType = ModContent.BuffType<Buffs.StoneBuff>();
			// Shoots a Stone Minion
			item.shoot = ModContent.ProjectileType<Projectiles.StoneMinion>();
			// Prevents the default item graphic from being used
			item.noUseGraphic = true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			// When shoot is called it should also spawn the staff
			Projectile.NewProjectile(position, Vector2.Zero, ModContent.ProjectileType<Projectiles.StoneStaffProjectile>(), 0, 0, item.owner);
			// Adds the buff
			player.AddBuff(item.buffType, 2);
			// spawns minion at mouse position
			position = Main.MouseWorld;
			return true;
		}

		public override void AddRecipes()
		{
			// Recipe Stone Block (36) @ Workbench
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.StoneBlock, 36);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			// Draws the glowing gem and effect as a glow mask
			Texture2D glow = mod.GetTexture("Items/StoneGlow");
			Rectangle sourceRectangle = Main.itemAnimations[item.type].GetFrame(glow);
			Vector2 offset = sourceRectangle.Size() * 0.5f;
			spriteBatch.Draw(glow, item.Center - Main.screenPosition - new Vector2(0, 18f), sourceRectangle, Color.White, rotation, offset, scale, SpriteEffects.None, 0f);
		}
	}
}
