﻿using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Creature;

namespace NeoServer.Game.Common.Contracts.Creatures;

public delegate bool PathFinder(IWalkableCreature creature, Location.Structs.Location target, FindPathParams options,
    ITileEnterRule tileEnterRule, out Direction[] directions);

public delegate void StartFollow(IWalkableCreature creature, ICreature following, FindPathParams fpp);

public delegate void ChangeSpeed(IWalkableCreature creature, ushort speed);

public delegate bool CanGoToDirection(ICreature creature, Location.Structs.Location location, Direction direction,
    ITileEnterRule rule);

public delegate void TeleportTo(IWalkableCreature creature, Location.Structs.Location location);

public delegate void Moved(IWalkableCreature creature, Location.Structs.Location fromLocation,
    Location.Structs.Location toLocation,
    ICylinderSpectator[] spectators);

public interface IWalkableCreature : ICreature
{
    ICreature Following { get; }
    bool HasNextStep { get; }
    bool IsFollowing { get; }
    ushort Speed { get; }
    public bool HasNoSpeed => Speed is 0;
    int StepDelay { get; }
    bool FirstStep { get; } //remove
    ITileEnterRule TileEnterRule { get; }
    FindPathParams PathSearchParams { get; }
    CooldownList Cooldowns { get; }
    event StartWalk OnStartedWalking;
    event StopWalk OnStoppedWalking;
    event TurnedToDirection OnTurnedToDirection;
    event StartFollow OnStartedFollowing;
    event ChangeSpeed OnChangedSpeed;
    event StopWalk OnCompleteWalking;
    event TeleportTo OnTeleported;
    event Moved OnCreatureMoved;

    /// <summary>
    ///     Decreases creature speed
    /// </summary>
    void DecreaseSpeed(ushort speed);

    /// <summary>
    ///     Increases creature speed
    /// </summary>
    /// <param name="speed"></param>
    void IncreaseSpeed(ushort speed);

    byte[] GetRaw(IPlayer playerRequesting);
    void OnMoved(IDynamicTile fromTile, IDynamicTile toTile, ICylinderSpectator[] spectators); //remove

    /// <summary>
    ///     Follow creature
    /// </summary>
    /// <param name="creature"></param>
    /// <param name="findPathParams"></param>
    void Follow(ICreature creature, FindPathParams findPathParams);

    void StopFollowing();

    /// <summary>
    ///     Stops walking
    /// </summary>
    void StopWalking();

    /// <summary>
    ///     Walks to a given location
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    bool WalkTo(Location.Structs.Location location);

    /// <summary>
    ///     Walks to a given location and call a action when finished
    /// </summary>
    bool WalkTo(Location.Structs.Location location, Action<ICreature> callbackAction, Direction[] path = null);

    /// <summary>
    ///     Walks to a sequence of direction
    /// </summary>
    /// <param name="directions"></param>
    /// <returns></returns>
    bool WalkTo(params Direction[] directions);

    /// <summary>
    ///     Teleport creature to a given location
    /// </summary>
    /// <param name="location"></param>
    void TeleportTo(Location.Structs.Location location);

    /// <summary>
    ///     Teleport creature to a given coordinate
    /// </summary>
    void TeleportTo(ushort x, ushort y, byte z);

    /// <summary>
    ///     Walk one random step
    /// </summary>
    /// <returns></returns>
    bool WalkRandomStep();

    /// <summary>
    ///     Get creature's next step direction
    /// </summary>
    /// <returns>Returns None when there is no next direction</returns>
    Direction NextStep { get; }

    /// <summary>
    ///     Sets current tile which creature is on
    /// </summary>
    /// <param name="tile"></param>
    void SetCurrentTile(IDynamicTile tile);

    void TurnTo(Direction direction);
    void Follow(ICreature creature);
    void CancelWalk();
    event StopWalk OnCancelledWalking;
    void AddStep(Direction direction);
    void ClearNextSteps();
    void SetFirstStep(bool firstStep);
    void AddSteps(Direction[] directions);
}