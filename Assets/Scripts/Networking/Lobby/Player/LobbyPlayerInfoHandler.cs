using UnityEngine;
using UnityEngine.Events;
using Mirror;
using Steamworks;

public class LobbyPlayerInfoHandler : NetworkBehaviour
{
    [SerializeField]
    private UnityEvent<string> OnPlayerSteamNameUpdated;

    [SyncVar ( hook = nameof ( HandleSteamIdUpdated ) )]
    private ulong m_steamId;

    #region Server

    public void SetSteamId ( ulong steamId )
    {
        Debug.Log ( $"SetSteamId() - steamId={steamId}" );
        m_steamId = steamId;
    }

    #endregion

    #region Client

    private void HandleSteamIdUpdated ( ulong oldSteamId, ulong newSteamId )
    {
        CSteamID cSteamId = new CSteamID ( newSteamId );

        OnPlayerSteamNameUpdated?.Invoke ( SteamFriends.GetFriendPersonaName ( cSteamId ) );
    }

    #endregion
}
