using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents
{
    public static string OnInputManagerUpdate = "OnInputManagerUpdate";
    public static string OnMouseUpdate = "OnMouseUpdate";
    public static string OnPlayerFire = "OnPlayerFire";
    public static string OnPlayerHealthUpdate = "OnPlayerHealthUpdate";

    public static string OnSetPlayerId = "OnSetPlayerId";
    public static string OnUpdatePlayerInfo = "OnUpdatePlayerInfo";
    public static string OnUpdatePositionData = "OnUpdatePositionData";
    public static string OnJoinRoom = "OnJoinRoom";
    public static string OnCreateRoom = "OnCreateRoom";
    public static string OnUpdateCreateRoomInfo = "OnUpdateCreateRoomInfo";

    public static string OnAiHealthUpdate = "OnAiHealthUpdate";
    public static string OnAiFireUpdate = "OnAiFireUpdate";
}
