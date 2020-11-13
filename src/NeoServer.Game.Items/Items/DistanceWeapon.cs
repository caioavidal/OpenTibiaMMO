﻿using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Combat.Calculations;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Body;
using NeoServer.Game.Creatures.Combat.Attacks;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Enums.Players;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Immutable;

namespace NeoServer.Game.Items.Items
{
  
    public class DistanceWeapon : MoveableItem, IDistanceWeaponItem
    {
        public DistanceWeapon(IItemType type, Location location) : base(type, location)
        {
        }
        public ImmutableHashSet<VocationType> AllowedVocations { get; }

        public byte ExtraAttack => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Attack);
        public byte ExtraHitChance => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.HitChance);
        public byte Range => Metadata.Attributes.GetAttribute<byte>(Enums.ItemAttribute.Range);

        public static bool IsApplicable(IItemType type) => type.Attributes.GetAttribute(Enums.ItemAttribute.WeaponType) == "distance" && !type.HasFlag(Enums.ItemFlag.Stackable);

        public bool Use(ICombatActor actor, ICombatActor enemy, out CombatAttackType combatType)
        {
            var result = false;
            combatType = new CombatAttackType();

            if (!(actor is IPlayer player)) return false;

            if (player?.Inventory[Slot.Ammo] == null) return false;

            if (!(player?.Inventory[Slot.Ammo] is AmmoItem ammo)) return false;

            if (ammo.AmmoType != Metadata.AmmoType) return false;

            if (ammo.Amount <= 0) return false;

            var hitChance = (byte)(DistanceHitChanceCalculation.CalculateFor2Hands(player.Skills[player.SkillInUse].Level, Range) + ExtraHitChance);

            var maxDamage = actor.CalculateAttackPower(0.09f, (ushort)(ammo.Attack + ExtraAttack));

            combatType.ShootType = ammo.ShootType;

            var combat = new CombatAttackValue(actor.MinimumAttackPower, maxDamage, Range, hitChance, DamageType.Physical);

            if (DistanceCombatAttack.Instance.TryAttack(actor, enemy, combat, out var damage))
            {
                enemy.ReceiveAttack(enemy, damage);
                result = true;
            }

            maxDamage = actor.CalculateAttackPower(0.09f, (ushort)(ammo.ElementalDamage.Item2 + ExtraAttack));
            combat = new CombatAttackValue(actor.MinimumAttackPower, maxDamage, Range, hitChance, ammo.ElementalDamage.Item1);
            if (ammo.ElementalDamage != null && DistanceCombatAttack.Instance.TryAttack(actor, enemy, combat, out var elementalDamage))
            {
                combatType.DamageType = ammo.ElementalDamage.Item1;

                enemy.ReceiveAttack(enemy, elementalDamage);
                result = true;
            }

            if (result)
            {
                ammo.Reduce(1);
            }

            return result;
        }
    }
}
