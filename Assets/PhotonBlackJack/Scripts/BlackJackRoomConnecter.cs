using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

enum Scenes
{
    ConnectScene,
    PhotonBlackJackGameScene
}

public class BlackJackRoomConnecter : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI ResultText;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
    }

    public void OnButton()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions options = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(null, options);
        ResultText.text = "Room Make";
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StartGame();
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        PhotonNetwork.LoadLevel((int)Scenes.PhotonBlackJackGameScene);
    }
}

