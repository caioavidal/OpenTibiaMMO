﻿using NeoServer.Networking.Packets.Network;

namespace NeoServer.Networking.Packets.Incoming.Chat;

public class OpenChannelPacket : IncomingPacket
{
    public OpenChannelPacket(IReadOnlyNetworkMessage message)
    {
        ChannelId = message.GetUInt16();
    }

    public ushort ChannelId { get; set; }
}