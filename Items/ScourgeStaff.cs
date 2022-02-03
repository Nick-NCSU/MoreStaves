using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace MoreStaves.Items
{
	public class ScourgeStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scourge Staff");
			Tooltip.SetDefault("Summons a scourge minion to fight for you");
			ItemID.Sets.GamepadWholeScreenUseRange[item.type] = true;
			ItemID.Sets.LockOnIgnoresCollision[item.type] = true;
		}

		public override void SetDefaults()
		{
			item.damage = 8;
			item.width = 40;
			item.height = 40;
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
			item.buffType = ModContent.BuffType<Buffs.ScourgeBuff>();
			item.shoot = ModContent.ProjectileType<Projectiles.ScourgeMinion>();
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			player.AddBuff(item.buffType, 2);
			position = Main.MouseWorld;
			return true;
		}

		public override void AddRecipes()
		{
			Mod calamity = ModLoader.GetMod("CalamityMod");
			if (calamity != null)
			{
				ModRecipe recipe = new ModRecipe(mod);
				recipe.AddIngredient(calamity.ItemType("VictoryShard"), 4);
				recipe.AddIngredient(ItemID.SandBlock, 10);
				recipe.AddTile(TileID.WorkBenches);
				recipe.SetResult(this);
				recipe.AddRecipe();
			}
		}
	}
}
