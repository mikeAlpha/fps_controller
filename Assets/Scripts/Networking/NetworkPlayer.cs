using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    public string player_id;
    public bool IsMasterPlayer = false;

    protected void OnEnable()
    {
        EventHandler.RegisterEvent<string>(GameEvents.OnSetPlayerId, SetPlayerId);
    }

    protected void OnDisable()
    {
        EventHandler.UnregisterEvent<string>(GameEvents.OnSetPlayerId, SetPlayerId);
    }

    private void SetPlayerId(string player_id)
    {
        this.player_id = player_id;
    }
}
