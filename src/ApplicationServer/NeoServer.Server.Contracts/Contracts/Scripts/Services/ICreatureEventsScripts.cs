﻿using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Server.Common.Contracts.Scripts.Services;

public interface ICreatureEventsScripts
{
    void PlayerExtendedOpcodeHandle(IPlayer player, byte opcode, string buffer);

    //void CreatureEventExecuteOnPlayerLogin(IPlayer player);

    //void CreatureEventExecuteOnPlayerLogout(IPlayer player);

    //void CreatureEventExecuteOnThink(ICreature creature, int interval);
}