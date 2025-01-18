using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.UseCase;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using Serilog;

namespace NeoServer.Game.Creatures.UseCases.Monster;

public class CreateMonsterOrSummonUseCase(
    ILogger logger,
    ICreatureFactory creatureFactory,
    IMap map,
    ICreatureGameInstance creatureGameInstance) : ICreateMonsterOrSummonUseCase
{
    public IMonster Invoke(string name, Location location, bool extended = false, bool forced = false, ICreature master = null)
    {
        var monster = master is null ? creatureFactory.CreateMonster(name) : creatureFactory.CreateSummon(name, master);
        if (!monster)
        {
            logger.Error("Monster {Name} does not exists.", name);
            return null;
        }

        var tileToBorn = map[location];
        if (tileToBorn is IDynamicTile tile && (tile is { HasCreature: false } || (!extended && forced)))
        {
            if (tile.HasFlag(TileFlags.ProtectionZone))
            {
                return null;
            }

            BornMonster(monster, location);
            return monster;
        }

        foreach (var neighbour in extended ? location.ExtendedNeighbours : location.Neighbours)
        {
            if (map[neighbour] is not IDynamicTile { HasCreature: false }) continue;

            BornMonster(monster, location);
            return monster;
        }

        if (!forced) return null;
        BornMonster(monster, location);
        return monster;
    }

    private void BornMonster(IMonster monster, Location location)
    {
        monster.Born(location);
        map.PlaceCreature(monster);
        creatureGameInstance.Add(monster);
    }
}