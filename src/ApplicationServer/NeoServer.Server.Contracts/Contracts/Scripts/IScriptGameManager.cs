﻿using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Server.Common.Contracts.Scripts;

public interface IScriptGameManager
{
    void Start();
    bool PlayerSaySpell(IPlayer player, SpeechType type, string text);
    bool PlayerUseItem(IPlayer player, Location pos, byte stackpos, byte index, IItem item, IThing target = null);
    bool PlayerUseItem(IPlayer player, Location fromPos, Location toPos, byte toStackPos, IItem item, IThing target = null, bool isHotkey = false);
    bool HasAction(IItem item);
}