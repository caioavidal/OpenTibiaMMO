﻿using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Application.Features.Item.Container.OpenContainer;

public class PlayerOpenedContainerEventHandler: IEventHandler
{
    private readonly IGameServer game;

    public PlayerOpenedContainerEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(IPlayer player, byte containerId, IContainer container)
    {
        SendContainerPacket(player, containerId, container);
    }

    private void SendContainerPacket(IPlayer player, byte containerId, IContainer container)
    {
        if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        connection.OutgoingPackets.Enqueue(new OpenContainerPacket(container, containerId));
        connection.Send();
    }
}