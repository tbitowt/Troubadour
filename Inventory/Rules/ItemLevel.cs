namespace T4.Plugins.Troubadour;

public static partial class Inventory
{
    private static ItemTextLine CreateItemLevel() => new()
    {
        Show = (item, features) =>
        {
            if (!features.ItemLevelEnabled)
                return false;
            if (item.ItemPower == 0)
                return false;
            if (item.IsAspectItem())
                return false;
            if (item.ItemSno.GemType != GemType.None)
                return false;
            if (item.Location is ItemLocation.PlayerBackpack or ItemLocation.Stash)
                return true;
            if (item.ItemSno.ItemUseType == ItemUseType.DungeonKey)
                return true;

            return item.IsEquippedTemp();
        },
        Text = (item, features) =>
        {
            if (item.ItemSno.ItemUseType == ItemUseType.DungeonKey)
                return $"T{item.SigilLevel}";

            var itemPower = $"{item.ItemPower}";
            if (item.UpgradeCount > 0)
                itemPower += $"+{item.UpgradeCount * 5}";
            if (!features.ItemQualityModifierEnabled)
                return $"i{itemPower}";

            return item.QualityModifier switch
            {
                ItemQualityModifier.Sacred => $"s{itemPower}",
                ItemQualityModifier.Ancestral => $"a{itemPower}",
                _ => $"i{itemPower}",
            };
        },
    };
}

public sealed partial class InventoryFeatures
{
    public InventoryFeatures ItemLevel(bool enabled = true)
    {
        AddTextLine(new BooleanFeatureResource
        {
            NameOf = nameof(ItemLevelEnabled), 
            DisplayText = () => Translation.Translate(Plugin, "iLvl"),
            Getter = () => ItemLevelEnabled, 
            Setter = v => ItemLevelEnabled = v,
        });
        ItemLevelEnabled = enabled;
        return this;
    }

    public InventoryFeatures ItemQualityModifier(bool enabled = true)
    {
        AddTextLine(new BooleanFeatureResource
        {
            NameOf = nameof(ItemQualityModifierEnabled),
            DisplayText = () => $"{Translation.Translate(Plugin, "iLvl")} {Translation.Translate(Plugin, "quality prefix: s = sacred, a = ancestral")}",
            Getter = () => ItemQualityModifierEnabled,
            Setter = v => ItemQualityModifierEnabled = v,
        });
        ItemQualityModifierEnabled = enabled;
        return this;
    }
}