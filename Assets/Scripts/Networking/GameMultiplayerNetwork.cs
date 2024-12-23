using System;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;
using Unity.VisualScripting;

public class GameMultiplayerNetwork
{
    private WebSocket ws;
    public bool IsConnected = false;

    public GameMultiplayerNetwork() {

        EventHandler.RegisterEvent<string>(GameEvents.OnUpdatePlayerInfo, SendPlayerInfoConnect);
        EventHandler.RegisterEvent<Vector3,string,string>(GameEvents.OnUpdatePositionData, SendPositionData);
        EventHandler.RegisterEvent<string, string>(GameEvents.OnJoinRoom, JoinRoom);
        EventHandler.RegisterEvent<string, string>(GameEvents.OnCreateRoom, CreateRoom);

        ws = new WebSocket("ws://localhost:6969/SyncPosition");

        ws.OnMessage += (sender, e) =>
        {
            UnityThread.executeInUpdate(() => 
            {
                ProcessServerResponse(e.Data);
            });
        };

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("Connected to server");
            IsConnected = true;
        };

        ws.OnError += (sender, e) =>
        {
            Debug.Log("Connection Error!! " + e.Message);
        };

        ws.Connect();

    }

    private void ProcessServerResponse(string response)
    {
        Debug.Log($"{response}");
        try
        {
            var data = JsonConvert.DeserializeObject<CreateRoomUpdateInfo>(response);

            Debug.Log(data.message);

            EventHandler.ExecuteEvent(GameEvents.OnUpdateCreateRoomInfo, data);
        }
        catch(Exception e)
        {
            Debug.LogError("Error===" + e.Message);
        }
    }

    private void SendPlayerInfoConnect(string player_id)
    {
        var data = new { player_name = player_id };
        string msg = JsonConvert.SerializeObject(data);

        Debug.Log(msg);

        if(ws.ReadyState == WebSocketState.Open)
            ws.Send(msg);
    }

    private void SendPositionData(Vector3 pos, string objectId, string roomId)
    {
        PositionData positionData = new PositionData()
        {
            objectId = objectId,
            roomId = roomId,
            eventType = "PositionUpdate",
            x = pos.x,
            y = pos.y,
            z = pos.z
        };

        string jsonData = JsonConvert.SerializeObject(positionData);

        //Debug.Log(jsonData);

        ws.Send(jsonData);
    }

    private void JoinRoom(string object_Id, string room_Id)
    {
        var joinRoomRequest = new
        {
            objectId = object_Id,
            roomId = room_Id,
            eventType = "JoinRoom"
        };

        string jsonData = JsonConvert.SerializeObject(joinRoomRequest);
        ws.Send(jsonData);
    }

    public void CreateRoom(string object_Id, string room_Id)
    {
        var createRoomRequest = new
        {
            objectId = object_Id,
            roomId = room_Id,
            eventType = "CreateRoom"
        };

        string jsonData = JsonConvert.SerializeObject(createRoomRequest);
        ws.Send(jsonData);
    }


    [System.Serializable]
    public class PositionData
    {
        public string objectId;
        public string roomId;
        public string eventType;
        public float x;
        public float y;
        public float z;
    }

    [System.Serializable]
    public class GenericEventClass<T>
    {
        public string eventType { get; set; }
        public T param;
    }

    [System.Serializable]
    public class CreateRoomUpdateInfo
    {

        public string message { get; set; }
        public string roomId { get; set; }
        public string ownerId {  get; set; }
    }

    //public string clientId = "Player1"; 
    //public GameObject gameObjectToSync;  

    //void Start()
    //{
    //    ws = new WebSocket("ws://localhost:6969/ws");

    //    ws.OnMessage += (sender, e) =>
    //    {
    //        Debug.Log("Message received: " + e.Data);

    //        Dictionary<string, Vector3> positions = JsonConvert.DeserializeObject<Dictionary<string, Vector3>>(e.Data);

    //        if (positions.ContainsKey(clientId))
    //        {
    //            Vector3 newPosition = positions[clientId];
    //            gameObjectToSync.transform.position = newPosition;
    //        }
    //    };

    //    ws.Connect();
    //    StartCoroutine(SendPositionData());
    //}

    //IEnumerator SendPositionData()
    //{
    //    while (true)
    //    {
    //        Vector3 position = gameObjectToSync.transform.position;
    //        Dictionary<string, object> message = new Dictionary<string, object>
    //        {
    //            { "id", clientId },
    //            { "position", new Dictionary<string, float>
    //                {
    //                    { "x", position.x },
    //                    { "y", position.y },
    //                    { "z", position.z }
    //                }
    //            }
    //        };

    //        string jsonMessage = JsonConvert.SerializeObject(message);

    //        Debug.Log(jsonMessage);

    //        ws.Send(jsonMessage);

    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}

    //void OnDestroy()
    //{
    //    if (ws != null)
    //    {
    //        ws.Close();
    //    }
    //}
}

