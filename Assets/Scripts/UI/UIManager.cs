using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameMultiplayerNetwork;

public class UIManager : MonoBehaviour
{
    [Header("Create Panel Section")]
    [SerializeField] private Button create_btn;
    [SerializeField] private TMP_InputField player_name;
    [SerializeField] private TMP_InputField room_id;

    [Header("Main Panel Sections")]
    [SerializeField] private Button create_panel_btn;
    [SerializeField] private Button join_panel_btn;

    [Header("All Panels")]
    [SerializeField] private GameObject JoinRoomPanel;
    [SerializeField] private GameObject CreateRoomPanel;
    [SerializeField] private GameObject RoomInfoPanel;
    [SerializeField] private GameObject MainPanel;


    private void OnEnable()
    {
        EventHandler.RegisterEvent<CreateRoomUpdateInfo>(GameEvents.OnUpdateCreateRoomInfo, OnCreateRoomUpdateInfo);
    }

    private void OnDisable()
    {
        EventHandler.UnregisterEvent<CreateRoomUpdateInfo>(GameEvents.OnUpdateCreateRoomInfo, OnCreateRoomUpdateInfo);
    }

    private void Start()
    {
        create_btn.onClick.RemoveAllListeners();
        create_btn.onClick.AddListener(() => 
        { 
            if(string.IsNullOrEmpty(player_name.text) || string.IsNullOrEmpty(room_id.text))
            {
                Debug.LogError("Please enter player name and room id");
                return;
            }

            MultiplayerManager.CreateRoom(player_name.text, room_id.text);
        });

        create_panel_btn.onClick.RemoveAllListeners();
        create_panel_btn.onClick.AddListener(() =>
        {
            CreateRoomPanel.SetActive(true);
            MainPanel.SetActive(false);
        });

        join_panel_btn.onClick.RemoveAllListeners();
        join_panel_btn.onClick.AddListener(() =>
            {
                JoinRoomPanel.SetActive(true);
                MainPanel.SetActive(false);
            });
    }

    private void OnCreateRoomUpdateInfo(CreateRoomUpdateInfo data)
    {
        RoomInfoPanel.SetActive(true);
        CreateRoomPanel.SetActive(false);
    }
}
