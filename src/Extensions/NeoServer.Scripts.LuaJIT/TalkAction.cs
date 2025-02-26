﻿using NeoServer.Game.Common.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Scripts.LuaJIT.Interfaces;
using Serilog;

namespace NeoServer.Scripts.LuaJIT;

public class TalkAction : Script, ITalkAction
{
    private string separator = "\"";

    private string words;
    //private account.GroupType groupType = account.GroupType.GROUP_TYPE_NONE;

    private ILogger _logger;

    public TalkAction(LuaScriptInterface context, ILogger logger) : base(context)
    {
        _logger = logger;
    }

    public bool ExecuteSay(IPlayer player, string words, string param, SpeechType type)
    {
        // onSay(player, words, param, type)
        if (!GetScriptInterface().InternalReserveScriptEnv())
        {
            _logger.Error($"[TalkAction::ExecuteSay - Player {player.Name} words {GetWords()}] " +
                              $"Call stack overflow. Too many lua script calls being nested. Script name {GetScriptInterface().GetLoadingScriptName()}");
            return false;
        }

        var scriptInterface = GetScriptInterface();
        var scriptEnvironment = scriptInterface.InternalGetScriptEnv();
        scriptEnvironment.SetScriptId(GetScriptId(), GetScriptInterface());

        var luaState = GetScriptInterface().GetLuaState();
        GetScriptInterface().PushFunction(GetScriptId());

        LuaScriptInterface.PushUserdata(luaState, player);
        LuaScriptInterface.SetMetatable(luaState, -1, "Player");

        LuaScriptInterface.PushString(luaState, words);
        LuaScriptInterface.PushString(luaState, param);
        //Lua.PushNumber(luaState, (double)type);

        return GetScriptInterface().CallFunction(3);
    }

    public string GetWords()
    {
        return words;
    }

    public void SetWords(List<string> newWords)
    {
        foreach (var word in newWords)
        {
            if (!string.IsNullOrEmpty(words)) words += ", ";
            words += word;
        }
    }

    public string GetSeparator()
    {
        return separator;
    }

    public void SetSeparator(string sep)
    {
        separator = sep;
    }

    //public void SetGroupType(account.GroupType newGroupType)
    //{
    //    groupType = newGroupType;
    //}

    //public account.GroupType GetGroupType()
    //{
    //    return groupType;
    //}

    public override string GetScriptTypeName()
    {
        return "onSay";
    }
}