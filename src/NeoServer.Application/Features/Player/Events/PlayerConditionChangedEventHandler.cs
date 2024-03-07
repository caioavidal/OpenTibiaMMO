﻿using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Parsers;
using NeoServer.Networking.Packets.Outgoing.Player;

namespace NeoServer.Application.Features.Player.Events;

public class PlayerConditionChangedEventHandler : IEventHandler
{
    private readonly IGameServer game;

    public PlayerConditionChangedEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(ICreature creature, ICondition c)
    {
        if (creature is not IPlayer player) return;
        if (!game.CreatureManager.GetPlayerConnection(creature.CreatureId, out var connection)) return;

        ushort icons = 0;
        foreach (var condition in player.Conditions) icons |= (ushort)ConditionIconParser.Parse(condition.Key);

        connection.OutgoingPackets.Enqueue(new ConditionIconPacket(icons));
        connection.Send();
    }
}