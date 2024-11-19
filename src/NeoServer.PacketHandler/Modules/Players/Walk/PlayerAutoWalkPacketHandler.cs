﻿using Mediator;
using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Modules.Movement.Creature.Walk;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Players.Walk;

public class PlayerAutoWalkPacketHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly IMediator _mediator;

    public PlayerAutoWalkPacketHandler(IGameServer game, IMediator mediator)
    {
        _game = game;
        _mediator = mediator;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var autoWalk = new AutoWalkPacket(message);

        if (_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            _game.Dispatcher.AddEvent(new Event(() => _mediator.Send(new WalkCommand(player, autoWalk.Steps))));
    }
}