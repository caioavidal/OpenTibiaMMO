﻿using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Game.Combat.Attacks;

public abstract class CombatAttack : ICombatAttack
{
    public virtual bool TryAttack(ICombatActor actor, ICombatActor enemy, CombatAttackCalculationValue option,
        out CombatAttackParams combatParams)
    {
        combatParams = new CombatAttackParams();
        return false;
    }

    public static bool CalculateAttack(ICombatActor actor, ICombatActor enemy, CombatAttackCalculationValue option,
        out CombatDamage damage)
    {
        damage = new CombatDamage();

        var damageValue = (ushort)GameRandom.Random.NextInRange(option.MinDamage, option.MaxDamage);
        damage = new CombatDamage(damageValue, option.DamageType);

        return true;
    }
}