using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;

namespace MoreStaves.Items
{
	// Adds the crimtane staff to the game
	class CrimtaneStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crimtane Staff");
			Tooltip.SetDefault("Summons a crimtane minion to fight for you.");

			// Sets the item to use 30 frames with with 5 ticks per frame
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 30));

			// Allows targeting across screen 
			ItemID.Sets.GamepadWholeScreenUseRange[item.type] = true;
			ItemID.Sets.LockOnIgnoresCollision[item.type] = true;
		}

		public override void SetDefaults()
		{
			item.damage = 11;
			item.width = 80;
			item.height = 80;
			item.useTime = 36;
			item.useAnimation = 36;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.value = 10;
			item.rare = ItemRarityID.Blue;
			item.UseSound = SoundID.Item1;
			item.mana = 10;
			item.noMelee = true;
			item.summon = true;
			item.knockBack = 3f;
			// Spawns the Crimtane Buff
			item.buffType = ModContent.BuffType<Buffs.CrimtaneBuff>();
			// Shoots a crimtane minion
			item.shoot = ModContent.ProjectileType<Projectiles.CrimtaneMinion>();
			// Scale of the item
			item.scale = 0.4f;
			// Prevents the default item graphic from being used
			item.noUseGraphic = true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			// When shoot is called it should also spawn the staff
			Projectile.NewProjectile(position, Vector2.Zero, ModContent.ProjectileType<Projectiles.CrimtaneStaffProjectile>(), 0, 0, item.owner);
			// Adds the buff
			player.AddBuff(item.buffType, 2);
			// Spawns minion at mouse position
			position = Main.MouseWorld;
			return true;
		}

		public override void AddRecipes()
		{
			// Recipe Tissue Sample (6), Crimtane Bar (12) @ Demon Altar
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.TissueSample, 6);
			recipe.AddIngredient(ItemID.CrimtaneBar, 12);
			recipe.AddTile(TileID.DemonAltar);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
			// Draws the staff with shading and animated texture
			Texture2D texture = mod.GetTexture("Items/CrimtaneStaff");
			spriteBatch.Draw(texture, item.Center - Main.screenPosition + new Vector2(0, 2400 * 0.4f), Main.itemAnimations[item.type].GetFrame(texture), lightColor, 0f, texture.Size() * 0.5f, scale * 0.4f, SpriteEffects.None, 0f);
			return false;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
			// Draws the glowing gem and effect as a glow mask
			Texture2D glow = mod.GetTexture("Items/CrimtaneGlow");
			spriteBatch.Draw(glow, item.Center - Main.screenPosition + new Vector2(0, 2400 * 0.4f), Main.itemAnimations[item.type].GetFrame(glow), Color.White, 0f, glow.Size() * 0.5f, scale * 0.4f, SpriteEffects.None, 0f);
        }
    }
}
