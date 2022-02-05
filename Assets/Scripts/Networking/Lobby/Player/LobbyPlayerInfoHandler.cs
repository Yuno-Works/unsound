using Mirror;
using Steamworks;

public class LobbyPlayerInfoHandler : NetworkBehaviour
{
    [SyncVar ( hook = nameof ( HandleSteamIdUpdated ) )]
    private ulong steamId;

    #region Server

    public void SetSteamId ( ulong steamId ) => CmdSetSteamId ( steamId );

    [Command]
    private void CmdSetSteamId ( ulong steamId ) => this.steamId = steamId;

    #endregion

    #region Client

    private void HandleSteamIdUpdated ( ulong oldSteamId, ulong newSteamId ) => steamId = newSteamId;

    #endregion
}
