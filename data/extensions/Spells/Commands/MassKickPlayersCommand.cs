﻿using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Helpers;

namespace NeoServer.Extensions.Spells.Commands;

public class MassKickPlayerCommand : CommandSpell
{
    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        var ctx  = IoC.GetInstance<IGameCreatureManager>();
        var playerLogOutCommand = IoC.GetInstance<PlayerLogOutCommand>();
        
         foreach (var player in ctx.GetAllLoggedPlayers())
         {
             playerLogOutCommand.Execute(player, true);
         }

        error = InvalidOperation.None;
        return true;
    }
}