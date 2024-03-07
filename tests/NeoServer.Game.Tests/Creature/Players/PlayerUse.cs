﻿using FluentAssertions;
using NeoServer.Application.Features.Player.UseItem;
using NeoServer.Application.Features.Shared;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Tests.Helpers;
using NeoServer.Game.Tests.Helpers.Map;
using NeoServer.Game.Tests.Helpers.Player;
using NeoServer.Game.Tests.Server;
using NeoServer.Game.World.Models.Tiles;
using Xunit;

namespace NeoServer.Game.Tests.Creature.Players;

public class PlayerUse
{
    [Fact]
    public void Player_uses_food_when_close_to_it()
    {
        //arrange
        var player = PlayerTestDataBuilder.Build();

        var tile = (DynamicTile)MapTestDataBuilder.CreateTile(new Location(100, 100, 7));
        var secondTile = (DynamicTile)MapTestDataBuilder.CreateTile(new Location(101, 100, 7));

        var map = MapTestDataBuilder.Build(tile, secondTile);

        var food = ItemTestData.CreateFood(2);

        tile.AddCreature(player);
        secondTile.AddItem(food);

        var playerUseService =
            new PlayerUseService(new WalkToMechanism(GameServerTestBuilder.Build(map).Scheduler), map);

        //act
        playerUseService.Use(player, food, player);

        //assert
        secondTile.DownItems.Should().BeEmpty();
    }
}