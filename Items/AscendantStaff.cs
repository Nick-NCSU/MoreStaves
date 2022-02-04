using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MoreStaves.Items
{
	class AscendantStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ascendant Staff");
			Tooltip.SetDefault("Summons Cluster Bombs to fight for you.\nMore powerful attacks against targeted enemies.");
			ItemID.Sets.GamepadWholeScreenUseRange[item.type] = true;
			ItemID.Sets.LockOnIgnoresCollision[item.type] = true;
		}

		public override void SetDefaults()
		{
			item.damage = 200;
			item.width = 40;
			item.height = 40;
			item.useTime = 36;
			item.useAnimation = 36;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.value = 10;
			item.rare = ItemRarityID.Purple;
			item.UseSound = SoundID.Item1;
			item.mana = 10;
			item.noMelee = true;
			item.summon = true;
			item.knockBack = 3f;
			item.buffType = ModContent.BuffType<Buffs.AscendantBuff>();
			item.shoot = ModContent.ProjectileType<Projectiles.AscendantMinion>();
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			if(player.HasBuff(item.buffType))
            {
				return false;
            }
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
				recipe.AddIngredient(calamity.ItemType("AscendantSpiritEssence"), 2);
				recipe.AddIngredient(calamity.ItemType("CosmiliteBar"), 6);
				recipe.AddIngredient(ItemID.DD2ExplosiveTrapT3Popper, 1);
				recipe.AddIngredient(ItemID.Bomb, 396);
				recipe.AddTile(calamity.TileType("CosmicAnvil"));
				recipe.SetResult(this);
				recipe.AddRecipe();
			}
		}
	}
}
