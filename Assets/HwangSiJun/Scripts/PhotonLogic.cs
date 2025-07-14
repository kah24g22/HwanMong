using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

enum RoomInfo
{
    turn,

}

public class PhotonLogic : MonoBehaviourPunCallbacks
{
    [SerializeField] TextMeshProUGUI m_playerText;
    [SerializeField] List<int> m_myCards = new List<int>();
    [SerializeField] TextMeshProUGUI m_myCardText;
    [SerializeField] List<int> m_enemyCards = new List<int>();
    [SerializeField] TextMeshProUGUI m_enemyCardText;

    [SerializeField] TextMeshProUGUI m_turnText;

    private bool m_myTurn;
    private Hashtable props = new Hashtable();

    private Player m_enemy; // 나를 제외한 플레이어 목록
    private Player m_my;

    private void Awake()
    {
        m_enemy = PhotonNetwork.PlayerListOthers[0];
        m_my = PhotonNetwork.LocalPlayer;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_playerText.text = m_my.ActorNumber.ToString();
        InitTurn();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitTurn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            props[RoomInfo.turn.ToString()] = m_my.ActorNumber; // 마스터가 먼저 시작
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            m_myTurn = true;
        }
        else
        {
            m_myTurn = false;
        }
    }

    public void AwakeTurn()
    {
        if (m_myTurn)
        {
            m_turnText.text = "MyTurn";
        }
        else
        {
            m_turnText.text = "EnemyTurn";
        } 
    }

    [PunRPC]
    public void TurnChange()
    {
        if (m_myTurn)
        {
            props[RoomInfo.turn.ToString()] = m_enemy.ActorNumber;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            m_myTurn = false;
        }
        else
        {
            props[RoomInfo.turn.ToString()] = m_my.ActorNumber;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            m_myTurn = true;
        }
        AwakeTurn();
    }

    public void OnAddCardButton()
    {
        if (m_myTurn)
        {
            int i = Random.Range(0, 10);
            m_myCards.Add(i);
            photonView.RPC("TurnChange", RpcTarget.All);
        }
    }
}
