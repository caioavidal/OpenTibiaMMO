﻿using NeoServer.Data.Interfaces;
using NeoServer.Networking.Handlers.ClientVersion;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Outgoing.Login;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Configurations;

namespace NeoServer.Networking.Handlers.LogIn;

public class AccountLoginHandler : PacketHandler
{
    private readonly ClientProtocolVersion _clientProtocolVersion;
    private readonly IAccountRepository _repositoryNeo;
    private readonly ServerConfiguration _serverConfiguration;

    public AccountLoginHandler(IAccountRepository repositoryNeo, ServerConfiguration serverConfiguration,
        ClientProtocolVersion clientProtocolVersion)
    {
        _repositoryNeo = repositoryNeo;
        _serverConfiguration = serverConfiguration;
        _clientProtocolVersion = clientProtocolVersion;
    }

    public override async void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var account = new AccountLoginPacket(message);
        connection.SetXtea(account.Xtea);

        if (!_clientProtocolVersion.IsSupported(account.ProtocolVersion))
        {
            connection.Disconnect($"Client protocol version {account.ProtocolVersion} is not supported.");
            return;
        }

        if (account == null)
        {
            //todo: use option
            connection.Disconnect("Invalid account.");
            return;
        }

        if (!account.IsValid())
        {
            connection.Disconnect("Invalid account name or password."); //todo: use gameserverdisconnect
            return;
        }

        var foundedAccount = await _repositoryNeo.GetAccount(account.Account, account.Password);

        if (foundedAccount == null)
        {
            connection.Disconnect("Account name or password is not correct.");
            return;
        }

        if (foundedAccount.BanishedAt is not null)
        {
            var untilMessage = foundedAccount.BanishedEndAt.HasValue ? $" until {foundedAccount.BanishedEndAt!.Value.ToString("MM/dd/yyyy")}." : ".";
            
            connection.Disconnect($"Your account has been banished{untilMessage}\nReason: {foundedAccount.BanishmentReason}");
            return;
        }

        connection.Send(new CharacterListPacket(foundedAccount, _serverConfiguration.ServerName,
            _serverConfiguration.ServerIp, (ushort)_serverConfiguration.ServerGamePort));
    }
}