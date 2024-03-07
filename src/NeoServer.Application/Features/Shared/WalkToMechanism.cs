﻿using NeoServer.Application.Common.Contracts.Tasks;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Application.Features.Shared;

public class WalkToMechanism : IWalkToMechanism
{
    private readonly IScheduler _scheduler;

    public WalkToMechanism(IScheduler scheduler)
    {
        _scheduler = scheduler;
    }

    public void WalkTo(IPlayer player, Action action, Location toLocation, bool secondChance = false,
        Direction[] path = null)
    {
        if (!toLocation.IsNextTo(player.Location))
        {
            if (secondChance) return;

            Action<ICreature> callBack = _ =>
                _scheduler.AddEvent(new SchedulerEvent(player.StepDelay,
                    () => WalkTo(player, action, toLocation, true)));

            player.WalkTo(toLocation, callBack, path);
            return;
        }

        action?.Invoke();
    }
}