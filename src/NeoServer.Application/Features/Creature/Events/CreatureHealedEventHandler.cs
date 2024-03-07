﻿using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Networking.Packets.Outgoing.Creature;
using NeoServer.Networking.Packets.Outgoing.Player;

namespace NeoServer.Application.Features.Creature.Events;

public class CreatureHealedEventHandler : IEventHandler
{
    private readonly IGameServer game;
    private readonly IMap map;

    public CreatureHealedEventHandler(IMap map, IGameServer game)
    {
        this.map = map;
        this.game = game;
    }

    public void Execute(ICreature healedCreature, ICreature healer, ushort amount)
    {
        foreach (var spectator in map.GetPlayersAtPositionZone(healedCreature.Location))
        {
            if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;

            if (Equals(healedCreature, spectator)) //myself
                connection.OutgoingPackets.Enqueue(new PlayerStatusPacket((IPlayer)healedCreature));

            connection.OutgoingPackets.Enqueue(new CreatureHealthPacket(healedCreature));

            connection.Send();
        }
    }
}