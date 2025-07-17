using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class PhotonLogic : MonoBehaviourPunCallbacks
{
    enum RoomInfo
    {
        turn,
        player1Cards,
        player2Cards,
    }

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
        AwakeTurn();
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
        props = PhotonNetwork.CurrentRoom.CustomProperties;

        if ((int)props[RoomInfo.turn.ToString()] == m_my.ActorNumber)
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
            photonView.RPC("AddCard", RpcTarget.All, i);
            photonView.RPC("TurnChange", RpcTarget.All);
        }
    }

    [PunRPC]
    public void AddCard(int pCardValue)
    {
        if (m_myTurn)
        {
            m_myCards.Add(pCardValue);
        }
        else
        {
            m_enemyCards.Add(pCardValue);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            props[RoomInfo.player1Cards.ToString()] = m_myCards.ToArray();
            props[RoomInfo.player2Cards.ToString()] = m_enemyCards.ToArray();
        }
        else
        {
            props[RoomInfo.player1Cards.ToString()] = m_enemyCards.ToArray();
            props[RoomInfo.player2Cards.ToString()] = m_myCards.ToArray();

        }
        CardTextUpdate();

    }

    public void CardTextUpdate()
    {
        string str = null;
        string str2 = null;

        foreach (var card in m_myCards)
        {
            str += $"{card} - ";
        }
        foreach (var card in m_enemyCards)
        {
            str2 += $"{card} - ";
        }

        m_myCardText.text = str;
        m_enemyCardText.text = str2;
    }
}
