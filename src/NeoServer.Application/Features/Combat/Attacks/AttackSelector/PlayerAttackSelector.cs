using NeoServer.Application.Features.Combat.Attacks.DistanceAttack;
using NeoServer.Application.Features.Combat.Attacks.MeleeAttack;
using NeoServer.Game.Combat;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Weapons;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Item.Items.Weapons;

namespace NeoServer.Application.Features.Combat.Attacks.AttackSelector;

public sealed class PlayerAttackSelector
{
    public Span<AttackParameter> Select(IPlayer player)
    {
        if (player is null)
        {
            return Span<AttackParameter>.Empty;
        }

        var elementalDamage = CalculateElementalAttack(player);

        var parameters = new AttackParameter[1];

        parameters[0] = new()
        {
            MinDamage = player.MinimumAttackPower,
            MaxDamage = player.MaximumAttackPower,
            DamageType = DamageType.Melee,
            Name = GetAttackName(player),
            ShootType = GetShootType(player),
            ExtraAttack = elementalDamage,
            CooldownType = CooldownType.Combat
        };

        return parameters;
    }

    private static ShootType GetShootType(IPlayer player)
    {
        var weapon = player.Inventory.Weapon;
        var ammo = player.Inventory.Ammo;

        return weapon switch
        {
            DistanceWeapon distanceWeapon when distanceWeapon.CanShootAmmunition(player.Inventory.Ammo) =>
                ammo?.ShootType ?? ShootType.None,
            IThrowableWeapon throwableDistanceWeapon => throwableDistanceWeapon.Metadata.ShootType,
            MagicWeapon magicWeapon => magicWeapon.Metadata.ShootType,
            _ => ShootType.None
        };
    }

    private string GetAttackName(IPlayer player) =>
        player.Inventory.IsUsingDistanceWeapon
            ? nameof(DistanceWeaponAttackStrategy)
            : nameof(MeleeAttackStrategy);

    private ExtraAttack CalculateElementalAttack(ICombatActor aggressor)
    {
        if (aggressor.MaximumElementalAttackPower is 0) return default;

        if (aggressor is not IPlayer player) return default;

        var damageType = player.Inventory.TotalElementalAttack.DamageType;

        return new ExtraAttack
        {
            MinDamage = aggressor.MinimumAttackPower,
            MaxDamage = aggressor.MaximumElementalAttackPower,
            DamageType = damageType
        };
    }
}