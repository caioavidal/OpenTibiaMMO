﻿using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Combat.Attacks;

public static class FistCombatAttack
{
    public static CombatAttackParams CombatAttackParams => new(DamageType.Melee);

    public static bool Attack(ICombatActor actor, ICombatActor enemy, out CombatAttackParams combatParams)
    {
        combatParams = CombatAttackParams;

        if (actor is not IPlayer player) return false;

        var maxDamage = player.CalculateAttackPower(0.085f, 7);
        var combat = new CombatAttackCalculationValue(actor.MinimumAttackPower,
            maxDamage, DamageType.Melee);

        if (!MeleeCombatAttack.CalculateAttack(actor, enemy, combat, out var damage)) return false;

        enemy.ReceiveAttack(actor, damage);

        return true;
    }
}