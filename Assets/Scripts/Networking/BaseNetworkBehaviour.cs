using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNetworkBehaviour : MonoBehaviour
{
    protected virtual void Start()
    {
        //OnCreateRoom();
    }

    protected virtual void Update()
    {
        //OnPositionUpdate();
    }

    protected virtual void OnPositionUpdate()
    {
        EventHandler.ExecuteEvent<Vector3, string, string>(GameEvents.OnUpdatePositionData, transform.position, gameObject.name, "room1");
    }

    protected virtual void OnJoinRoom()
    {
        EventHandler.ExecuteEvent<string, string>(GameEvents.OnJoinRoom, gameObject.name, "room1");
    }
    protected virtual void OnCreateRoom()
    {
        EventHandler.ExecuteEvent<string, string>(GameEvents.OnCreateRoom, gameObject.name, "room1");
    }

}
