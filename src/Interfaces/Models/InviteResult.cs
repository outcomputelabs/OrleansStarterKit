namespace Grains.Models
{
    public enum InviteResult
    {
        Success,

        SenderIsNotPartyLeader,
        PartyIsFull,
        PlayerAlreadyInParty,
        PlayerAlreadyInvited
    }
}
