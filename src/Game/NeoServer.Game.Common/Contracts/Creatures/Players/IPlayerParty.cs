﻿namespace NeoServer.Game.Common.Contracts.Creatures.Players
{
    public delegate void InviteToParty(IPlayer leader, IPlayer invited, IParty party);

    public delegate void RevokePartyInvite(IPlayer leader, IPlayer invited, IParty party);

    public delegate void RejectPartyInvite(IPlayer invited, IParty party);

    public delegate void JoinParty(IPlayer player, IParty party);

    public delegate void LeaveParty(IPlayer player, IParty party);

    public delegate void PassPartyLeadership(IPlayer leader, IPlayer newLeader, IParty party);
    public interface IPlayerParty
    {
        IParty Party { get; }
        bool IsInParty { get; }
        void PartyEmptyHandler();
        void InviteToParty(IPlayer invitedPlayer, IParty party);
        void AddPartyInvite(IPlayer @from, IParty party);
        void RejectInvite();
        void RevokePartyInvite(IPlayer invitedPlayer);
        Result LeaveParty();
        void JoinParty(IParty party);
        void PassPartyLeadership(IPlayer player);
        event InviteToParty OnInviteToParty;
        event InviteToParty OnInvitedToParty;
        event RevokePartyInvite OnRevokePartyInvite;
        event RejectPartyInvite OnRejectedPartyInvite;
        event JoinParty OnJoinedParty;
        event LeaveParty OnLeftParty;
        event PassPartyLeadership OnPassedPartyLeadership;
    }
}