using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    
    [SerializeField] private NetworkPlayer player;

    private void Start()
    {
        GameMultiplayerNetwork gameMultiplayerNetwork = new GameMultiplayerNetwork();
        if(gameMultiplayerNetwork.IsConnected)
        {
            var ply = Instantiate(player);
        }
    }
}
