using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;


namespace MoreStaves.Items
{
	// Adds the scourge staff to the game
	public class ScourgeStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scourge Staff");
			Tooltip.SetDefault("Summons a scourge minion to fight for you");

			// Sets the item to use 30 frames with with 5 ticks per frame
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 30));

			// Allows targeting across screen
			ItemID.Sets.GamepadWholeScreenUseRange[item.type] = true;
			ItemID.Sets.LockOnIgnoresCollision[item.type] = true;
		}

		public override void SetDefaults()
		{
			item.damage = 8;
			item.width = 20;
			item.height = 20;
			item.useTime = 36;
			item.useAnimation = 36;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = 10;
			item.rare = ItemRarityID.White;
			item.UseSound = SoundID.Item1; 
			item.mana = 1;
			item.noMelee = true;
			item.summon = true;
			item.knockBack = 5f;
			// Spawns the Scourge Buff
			item.buffType = ModContent.BuffType<Buffs.ScourgeBuff>();
			// Shoots a Scourge Minion
			item.shoot = ModContent.ProjectileType<Projectiles.ScourgeMinion>();
			// Prevents the default item graphic from being used
			item.noUseGraphic = true;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			// When shoot is called it should also spawn the staff
			Projectile.NewProjectile(position, Vector2.Zero, ModContent.ProjectileType<Projectiles.ScourgeStaffProjectile>(), 0, 0, item.owner);
			// Adds the buff
			player.AddBuff(item.buffType, 2);
			// spawns minion at mouse position
			position = Main.MouseWorld;
			return true;
		}

		public override void AddRecipes()
		{
			// Checks if calamity is loaded
			Mod calamity = ModLoader.GetMod("CalamityMod");
			if (calamity != null)
			{
				// Recipe Victory Shard (4), Sand Block (10) @ Workbench
				ModRecipe recipe = new ModRecipe(mod);
				recipe.AddIngredient(calamity.ItemType("VictoryShard"), 4);
				recipe.AddIngredient(ItemID.SandBlock, 10);
				recipe.AddTile(TileID.WorkBenches);
				recipe.SetResult(this);
				recipe.AddRecipe();
			}
		}

		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			// Draws the glowing gem and effect as a glow mask
			Texture2D glow = mod.GetTexture("Items/ScourgeGlow");
			Rectangle sourceRectangle = Main.itemAnimations[item.type].GetFrame(glow);
			Vector2 offset = sourceRectangle.Size() * 0.5f;
			spriteBatch.Draw(glow, item.Center - Main.screenPosition - new Vector2(0, 18f), sourceRectangle, Color.White, rotation, offset, scale, SpriteEffects.None, 0f);
		}
	}
}
