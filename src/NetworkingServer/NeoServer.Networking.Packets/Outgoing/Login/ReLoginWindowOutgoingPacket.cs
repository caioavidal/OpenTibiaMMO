﻿using NeoServer.Networking.Packets.Network;

namespace NeoServer.Networking.Packets.Outgoing.Login;

public class ReLoginWindowOutgoingPacket : OutgoingPacket
{
    public override void WriteToMessage(INetworkMessage message)
    {
        message.AddByte((byte)GameOutgoingPacketType.ReLoginWindow);
    }
}