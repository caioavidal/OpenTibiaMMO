﻿using NeoServer.Application.Features.Decay;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Server.Jobs.Items;

public class GameItemJob
{
    private const ushort EVENT_CHECK_ITEM_INTERVAL = 1000;
    private readonly IGameServer _game;
    private readonly IItemDecayTracker _itemDecayTracker;

    public GameItemJob(IGameServer game, IItemDecayTracker itemDecayTracker)
    {
        _game = game;
        _itemDecayTracker = itemDecayTracker;
    }

    public void StartChecking()
    {
        _game.Scheduler.AddEvent(new SchedulerEvent(EVENT_CHECK_ITEM_INTERVAL, StartChecking));

        _itemDecayTracker.DecayExpiredItems();
    }
}