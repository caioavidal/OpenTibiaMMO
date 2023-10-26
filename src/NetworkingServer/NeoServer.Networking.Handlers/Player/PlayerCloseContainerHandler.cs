﻿using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerCloseContainerHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PlayerCloseContainerHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var containerId = message.GetByte();
        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        _game.Dispatcher.AddEvent(new Event(() => player.Containers.CloseContainer(containerId)));
    }
}