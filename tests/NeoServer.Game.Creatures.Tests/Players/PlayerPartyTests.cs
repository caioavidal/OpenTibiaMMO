﻿using FluentAssertions;
using Moq;
using NeoServer.Game.Chats;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Creatures.Model.Players;
using NeoServer.Game.Tests.Helpers;
using Xunit;

namespace NeoServer.Game.Creatures.Tests.Players
{
    public class PlayerPartyTests
    {
        [Fact]
        public void Leader_cannot_invite_player_that_is_already_on_the_party()
        {
            //arrange
            var sut = PlayerTestDataBuilder.Build(hp: 100);
            var friend = PlayerTestDataBuilder.Build(hp: 100);


            var partyChannel = new ChatChannel(1, "party channel");
            var party = new Party(sut, partyChannel);

            sut.PlayerParty.InviteToParty(friend, party);
            friend.PlayerParty.JoinParty(party);
            
            using var monitor = sut.PlayerParty.Monitor();
            
            //act
            sut.PlayerParty.InviteToParty(friend, party);
            
            //assert
            monitor.Should().NotRaise(nameof(sut.PlayerParty.OnInviteToParty));
        }
        
        [Fact]
        public void Leader_cannot_invite_player_that_was_already_invited()
        {
            //arrange
            var sut = PlayerTestDataBuilder.Build(hp: 100);
            var friend = PlayerTestDataBuilder.Build(hp: 100);

            var partyChannel = new ChatChannel(1, "party channel");
            var party = new Party(sut, partyChannel);

            sut.PlayerParty.InviteToParty(friend, party);
            using var monitor = sut.PlayerParty.Monitor();
            
            //act
            sut.PlayerParty.InviteToParty(friend, party);

            //assert
            monitor.Should().NotRaise(nameof(sut.PlayerParty.OnInviteToParty));
        }

        [Fact]
        public void Non_leaders_cannot_invite_to_a_party()
        {
            var sut = PlayerTestDataBuilder.Build(hp: 100);

            var leader = PlayerTestDataBuilder.Build(hp: 100);

            var invitedPlayer = PlayerTestDataBuilder.Build(hp: 100);

            var invited = false;

            var party = new Party(sut, new Mock<IChatChannel>().Object);

            leader.PlayerParty.InviteToParty(sut, party);

            leader.PlayerParty.OnInviteToParty += (by, playerInvited, party) =>
            {
                if (playerInvited == invitedPlayer) invited = true;
            };

            sut.PlayerParty.InviteToParty(invitedPlayer, party);

            Assert.False(invited);
        }

        [Fact]
        public void Player_cannot_invite_himself()
        {
            //arrange
            var sut = PlayerTestDataBuilder.Build(hp: 100);
            var party = new Party(sut, new Mock<IChatChannel>().Object);
            using var monitor = sut.PlayerParty.Monitor();
            //act
            sut.PlayerParty.InviteToParty(sut, party);

            //assert
            monitor.Should().NotRaise((nameof(sut.PlayerParty.OnInviteToParty)));
        }

        [Fact]
        public void Player_invites_to_a_party()
        {
            var sut = PlayerTestDataBuilder.Build(hp: 100);

            var invitedPlayer = PlayerTestDataBuilder.Build(hp: 100);

            var invited = false;

            sut.PlayerParty.OnInviteToParty += (by, playerInvited, party) =>
            {
                if (playerInvited == invitedPlayer) invited = true;
            };
            var party = new Party(sut, new Mock<IChatChannel>().Object);

            sut.PlayerParty.InviteToParty(invitedPlayer, party);

            Assert.True(invited);
        }

        [Fact]
        public void Player_cannot_reject_party_without_an_invite()
        {
            //arrange
            var sut = PlayerTestDataBuilder.Build(hp: 100);
            using var monitor = sut.PlayerParty.Monitor();

            //act
            sut.PlayerParty.RejectInvite();

            //assert
            monitor.Should().NotRaise(nameof(sut.PlayerParty.OnRejectedPartyInvite));
            sut.PlayerParty.Party.Should().BeNull();
        }

        [Fact]
        public void Player_rejects_party()
        {
            //arrange
            var sut = PlayerTestDataBuilder.Build(hp: 100);
            var leader = PlayerTestDataBuilder.Build(hp: 100);
            using var monitor = sut.PlayerParty.Monitor();

            var partyChannel = new ChatChannel(1, "party channel");
            var party = new Party(leader, partyChannel);
            leader.PlayerParty.InviteToParty(sut, party);

            //act
            sut.PlayerParty.RejectInvite();

            //assert
            monitor.Should().Raise(nameof(sut.PlayerParty.OnRejectedPartyInvite));
            sut.PlayerParty.Party.Should().BeNull();
        }

        [Fact]
        public void Leader_cannot_revoke_invite_if_player_already_rejected()
        {
            //arrange
            var friend = PlayerTestDataBuilder.Build(hp: 100);
            var friend2 = PlayerTestDataBuilder.Build(hp: 100);

            var sut = PlayerTestDataBuilder.Build(hp: 100);
            using var monitor = friend.PlayerParty.Monitor();

            var partyChannel = new ChatChannel(1, "party channel");
            var party = new Party(sut, partyChannel);

            sut.PlayerParty.InviteToParty(friend2, party);
            friend.PlayerParty.JoinParty(party);

            sut.PlayerParty.InviteToParty(friend, party);
            friend.PlayerParty.RejectInvite();

            //act
            sut.PlayerParty.RevokePartyInvite(friend);

            //assert
            monitor.Should().NotRaise(nameof(friend.PlayerParty.OnRevokePartyInvite));
        }

        [Fact]
        public void Leader_cannot_revoke_invite_if_already_accepted()
        {
            //arrange
            var friend = PlayerTestDataBuilder.Build(hp: 100);

            var sut = PlayerTestDataBuilder.Build(hp: 100);
            using var monitor = friend.PlayerParty.Monitor();

            var partyChannel = new ChatChannel(1, "party channel");
            var party = new Party(sut, partyChannel);

            sut.PlayerParty.InviteToParty(friend, party);
            friend.PlayerParty.JoinParty(party);

            //act
            sut.PlayerParty.RevokePartyInvite(friend);

            //assert
            monitor.Should().NotRaise(nameof(friend.PlayerParty.OnRevokePartyInvite));
        }

        [Fact]
        public void Leader_cannot_revoke_invite_if_party_is_over()
        {
            //arrange
            var friend = PlayerTestDataBuilder.Build(hp: 100);

            var sut = PlayerTestDataBuilder.Build(hp: 100);
            using var monitor = friend.PlayerParty.Monitor();

            var partyChannel = new ChatChannel(1, "party channel");
            var party = new Party(sut, partyChannel);

            sut.PlayerParty.InviteToParty(friend, party);
            friend.PlayerParty.JoinParty(party);
            friend.PlayerParty.LeaveParty();

            //act
            sut.PlayerParty.RevokePartyInvite(friend);

            //assert
            monitor.Should().NotRaise(nameof(friend.PlayerParty.OnRevokePartyInvite));
        }
        
        [Fact]
        public void Player_cannot_leave_party_that_is_not_in()
        {
            //arrange
            var leader = PlayerTestDataBuilder.Build(hp: 100);

            var sut = PlayerTestDataBuilder.Build(hp: 100);
            using var monitor = leader.PlayerParty.Monitor();

            var partyChannel = new ChatChannel(1, "party channel");
            var party = new Party(leader, partyChannel);

            leader.PlayerParty.InviteToParty(sut, party);

            var enemy = MonsterTestDataBuilder.Build();
            sut.SetAsEnemy(enemy);
            
            //act
            var result = sut.PlayerParty.LeaveParty();

            //assert
            result.Error.Should().Be(InvalidOperation.NotPossible);
            monitor.Should().NotRaise(nameof(leader.PlayerParty.OnLeftParty));
        }
        
        [Fact]
        public void Player_cannot_leave_party_when_in_fight()
        {
            //arrange
            var leader = PlayerTestDataBuilder.Build(hp: 100);

            var sut = PlayerTestDataBuilder.Build(hp: 100);
            using var monitor = leader.PlayerParty.Monitor();

            var partyChannel = new ChatChannel(1, "party channel");
            var party = new Party(leader, partyChannel);

            leader.PlayerParty.InviteToParty(sut, party);
            sut.PlayerParty.JoinParty(party);

            var enemy = MonsterTestDataBuilder.Build();
            sut.SetAsEnemy(enemy);
            
            //act
            var result = sut.PlayerParty.LeaveParty();

            //assert
            result.Error.Should().Be(InvalidOperation.CannotLeavePartyWhenInFight);
            monitor.Should().NotRaise(nameof(leader.PlayerParty.OnLeftParty));
        }
        [Fact]
        public void Player_leaves_party()
        {
            //arrange
            var leader = PlayerTestDataBuilder.Build(hp: 100);
            var friend = PlayerTestDataBuilder.Build(hp: 100);
            
            var sut = PlayerTestDataBuilder.Build(hp: 100);
            using var monitor = sut.PlayerParty.Monitor();

            var partyChannel = new ChatChannel(1, "party channel");
            var party = new Party(leader, partyChannel);

            leader.PlayerParty.InviteToParty(sut, party);
            leader.PlayerParty.InviteToParty(friend, party);
            
            sut.PlayerParty.JoinParty(party);
            friend.PlayerParty.JoinParty(party);
            
            //act
            var result = sut.PlayerParty.LeaveParty();

            //assert
            result.IsSuccess.Should().BeTrue();
            monitor.Should().Raise(nameof(leader.PlayerParty.OnLeftParty));
            leader.PlayerParty.Party.Members.Should().NotContain(sut);
        }
        [Fact]
        public void Leader_pass_leadership_when_leaves_party()
        {
            //arrange
            var sut = PlayerTestDataBuilder.Build(hp: 100);
            using var monitor = sut.PlayerParty.Monitor();

            var friend = PlayerTestDataBuilder.Build(hp: 100);
            var secondFriend = PlayerTestDataBuilder.Build(hp: 100);

            var partyChannel = new ChatChannel(1, "party channel");
            var party = new Party(sut, partyChannel);

            sut.PlayerParty.InviteToParty(secondFriend, party);
            sut.PlayerParty.InviteToParty(friend, party);
            
            secondFriend.PlayerParty.JoinParty(party);
            friend.PlayerParty.JoinParty(party);
            
            //act
            var result = sut.PlayerParty.LeaveParty();

            //assert
            result.IsSuccess.Should().BeTrue();
            monitor.Should().Raise(nameof(sut.PlayerParty.OnLeftParty));
            monitor.Should().Raise(nameof(sut.PlayerParty.OnPassedPartyLeadership));
            party.Members.Should().NotContain(sut);
            party.Leader.Should().NotBe(sut);
        }
        
        [Fact]
        public void Leader_do_not_pass_leadership_when_party_is_over()
        {
            //arrange
            var sut = PlayerTestDataBuilder.Build(hp: 100);
            using var monitor = sut.PlayerParty.Monitor();

            var friend = PlayerTestDataBuilder.Build(hp: 100);

            var partyChannel = new ChatChannel(1, "party channel");
            var party = new Party(sut, partyChannel);

            sut.PlayerParty.InviteToParty(friend, party);
            friend.PlayerParty.JoinParty(party);
            
            //act
            var result = sut.PlayerParty.LeaveParty();

            //assert
            result.IsSuccess.Should().BeTrue();
            monitor.Should().Raise(nameof(sut.PlayerParty.OnLeftParty));
            monitor.Should().NotRaise(nameof(sut.PlayerParty.OnPassedPartyLeadership));
            party.Members.Should().NotContain(sut);
            party.Leader.Should().NotBe(sut);
            party.IsOver.Should().BeTrue();
        }
    }
}