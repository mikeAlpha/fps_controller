using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerManager : MonoBehaviour
{
    
    [SerializeField] private NetworkPlayer player;


    private static List<NetworkPlayer> mPlayers = new List<NetworkPlayer>();

    private void Start()
    {
        GameMultiplayerNetwork gameMultiplayerNetwork = new GameMultiplayerNetwork();
        //if(gameMultiplayerNetwork.IsConnected)
        //{
        //    var ply = Instantiate(player);
        //}
    }

    public static void CreateRoom(string room_ID, string object_ID)
    {
        EventHandler.ExecuteEvent<string, string>(GameEvents.OnCreateRoom, object_ID, room_ID);
    }

    public static void JoinRoom(string object_ID, string room_ID)
    {
        EventHandler.ExecuteEvent<string, string>(GameEvents.OnJoinRoom, object_ID, room_ID);
    }

    public static void ChangePlayerId(string playerId)
    {
        EventHandler.ExecuteEvent<string>(GameEvents.OnSetPlayerId,playerId);
    }
}
