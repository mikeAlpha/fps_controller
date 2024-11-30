using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    public string player_id = "PLayer_1";

    private void Start()
    {
        //EventHandler.ExecuteEvent<string>(GameEvents.OnUpdatePlayerInfo, player_id);
    }
}
