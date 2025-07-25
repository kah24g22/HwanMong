﻿using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

enum RoomInfo
{
    turn,
    player1Cards,
    player2Cards,
    player1IsTurnOn,
    player2IsTurnOn,
    player1Point,
    player2Point,
}

public class MockScript : MonoBehaviourPunCallbacks
{
    private PhotonBlackJackLogic m_gameLogic;

    [SerializeField] BlackJackPlayer m_player1;
    [SerializeField] BlackJackPlayer m_player2;

    // UI 요소 연결
    [SerializeField] private TextMeshProUGUI m_myNameText;
    [SerializeField] private TextMeshProUGUI m_enemyNameText;
    [SerializeField] private TextMeshProUGUI m_myScoreText;
    [SerializeField] private TextMeshProUGUI m_enemyScoreText;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI turnText; // 턴 표시 UI 추가
    [SerializeField] private Button hitButton;
    [SerializeField] private Button standButton;
    [SerializeField] private Button restartButton;

    [Header("Card Visuals")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform myHandTransform;
    [SerializeField] private Transform enemyHandTransform;
    [SerializeField] private float cardSpacing = 0.5f;

    private List<GameObject> myCardObjects = new List<GameObject>();
    private List<GameObject> enemyCardObjects = new List<GameObject>();

    private Dictionary<string, Sprite> cardSprites;

    private Player m_my;
    private Player m_enemy;
    private BlackJackPlayer m_myBlackJackPlayer;
    private BlackJackPlayer m_enemyBlackJackPlayer;

    private Hashtable props = new Hashtable();

    [SerializeField] Button[] buttons;

    private void Awake()
    {
        m_gameLogic = new PhotonBlackJackLogic(m_player1, m_player2);

        m_my = PhotonNetwork.LocalPlayer;
        m_enemy = PhotonNetwork.PlayerListOthers[0];

        m_gameLogic.OnCardDealt += CardUpdate;
        m_gameLogic.OnGameEnded += GameEnd;
        m_gameLogic.OnTurnChanged += TurnChanged;

        hitButton.onClick.AddListener(() => Hit());
        standButton.onClick.AddListener(() => Stand());
        restartButton.onClick.AddListener(() => StartGame());



        if (PhotonNetwork.IsMasterClient)
        {
            m_player1.Player = PhotonNetwork.LocalPlayer;
            m_player2.Player = PhotonNetwork.PlayerListOthers[0];
            m_myBlackJackPlayer = m_player1;
            m_enemyBlackJackPlayer = m_player2;
        }
        else
        {
            m_player1.Player = PhotonNetwork.PlayerListOthers[0];
            m_player2.Player = PhotonNetwork.LocalPlayer;
            m_myBlackJackPlayer = m_player2;
            m_enemyBlackJackPlayer = m_player1;
        }
    }

    void Start()
    {
        m_myNameText.text = m_my.ActorNumber.ToString();
        m_enemyNameText.text = m_enemy.ActorNumber.ToString();

        cardSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Cards/CuteCards");

        foreach (Sprite sprite in sprites)
        {
            cardSprites.Add(sprite.name, sprite);
        }
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            props = PhotonNetwork.CurrentRoom.CustomProperties;
            Debug.Log(props[RoomInfo.player1IsTurnOn.ToString()]);
            Debug.Log(m_gameLogic.Player1.IsTurnOn);
            Debug.Log(props[RoomInfo.player2IsTurnOn.ToString()]);
            Debug.Log(m_gameLogic.Player2.IsTurnOn);
        }
    }

    public void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC("StartGameLogic", RpcTarget.All, m_my);
        photonView.RPC("StartGameLogic", RpcTarget.All, m_enemy);
        props[RoomInfo.player1IsTurnOn.ToString()] = m_gameLogic.Player1.IsTurnOn;
        props[RoomInfo.player2IsTurnOn.ToString()] = m_gameLogic.Player2.IsTurnOn;
    }

    [PunRPC]
    public void StartGameLogic(Player pPlayer)
    {
        props[RoomInfo.player1Point.ToString()] = 0;
        props[RoomInfo.player2Point.ToString()] = 0;
        if (pPlayer == m_myBlackJackPlayer.Player)
        {
            foreach (GameObject card in myCardObjects) Destroy(card);
            myCardObjects.Clear();

            resultText.text = "";
            turnText.text = "";

            hitButton.interactable = true;
            standButton.interactable = true;

            if (PhotonNetwork.IsMasterClient)
                restartButton.gameObject.SetActive(false);

            m_gameLogic.StartGame(m_myBlackJackPlayer);

            m_gameLogic.DealCard(m_myBlackJackPlayer, true);
            m_gameLogic.DealCard(m_myBlackJackPlayer, false);

            props[RoomInfo.turn.ToString()] = m_gameLogic.Turn;
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        else if (pPlayer == m_enemy)
        {
            foreach (GameObject card in enemyCardObjects) Destroy(card);
            enemyCardObjects.Clear();
        }
        UpdateScores();
    }


    private void CardUpdate(Player pPlayer, Card card, bool isFaceUp)
    {
        photonView.RPC("HandleCardDealt", RpcTarget.All, pPlayer, card.suit, card.rank, isFaceUp);
    }
    private void GameEnd(string pMessage)
    {
        photonView.RPC("HandleGameEnded", RpcTarget.All, pMessage);
    }
    private void TurnChanged(PlayerTurn pCurrentPlayer)
    {
        photonView.RPC("HandleTurnChanged", RpcTarget.All, pCurrentPlayer);
    }

    [PunRPC]
    private void HandleCardDealt(Player pPlayer, string pCardSuit, string pCardRank, bool isFaceUp)
    {
        Debug.Log(pPlayer.ActorNumber);

        foreach (Card card in m_myBlackJackPlayer.Hand.Cards)
        {
            Debug.Log(card.suit + card.rank);

        }
        foreach (Card card in m_enemyBlackJackPlayer.Hand.Cards)
        {
            Debug.Log(card.suit + card.rank);

        }
        Transform parentTransform;
        List<GameObject> cardObjectsList;

        if (pPlayer == m_my)
        {
            parentTransform = myHandTransform;
            cardObjectsList = myCardObjects;

            GameObject newCardObject = Instantiate(cardPrefab, parentTransform);
            cardObjectsList.Add(newCardObject);

            newCardObject.transform.localPosition = new Vector3(cardSpacing * (cardObjectsList.Count - 1), 0, 0);

            newCardObject.GetComponent<SpriteRenderer>().sprite = GetCardSprite(pCardSuit, pCardRank);

            if(pPlayer == m_gameLogic.Player1.Player)
            {
                props[RoomInfo.player1Point.ToString()] = m_myBlackJackPlayer.Hand.CalculateValue();
                props[RoomInfo.player2Point.ToString()] = m_enemyBlackJackPlayer.Hand.CalculateValue();
            }
            else
            {
                props[RoomInfo.player2Point.ToString()] = m_myBlackJackPlayer.Hand.CalculateValue();
                props[RoomInfo.player1Point.ToString()] = m_enemyBlackJackPlayer.Hand.CalculateValue();
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        else if (pPlayer == m_enemy)
        {
            parentTransform = enemyHandTransform;
            cardObjectsList = enemyCardObjects;

            GameObject newCardObject = Instantiate(cardPrefab, parentTransform);
            cardObjectsList.Add(newCardObject);
            m_enemyBlackJackPlayer.Hand.AddCard(new Card(pCardSuit, pCardRank));

            newCardObject.transform.localPosition = new Vector3(cardSpacing * (cardObjectsList.Count - 1), 0, 0);

            if (isFaceUp)
            {
                newCardObject.GetComponent<SpriteRenderer>().sprite = GetCardSprite(pCardSuit, pCardRank);
            }
            else
            {
                newCardObject.GetComponent<SpriteRenderer>().sprite = cardSprites["Card_Back"];
            }
            if (pPlayer == m_gameLogic.Player1.Player)
            {
                props[RoomInfo.player1Point.ToString()] = m_enemyBlackJackPlayer.Hand.CalculateValue();
                props[RoomInfo.player2Point.ToString()] = m_myBlackJackPlayer.Hand.CalculateValue();
            }
            else
            {
                props[RoomInfo.player2Point.ToString()] = m_enemyBlackJackPlayer.Hand.CalculateValue();
                props[RoomInfo.player1Point.ToString()] = m_myBlackJackPlayer.Hand.CalculateValue();
            }
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }


        Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties[RoomInfo.player1Point.ToString()]);
        Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties[RoomInfo.player2Point.ToString()]);
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        UpdateScores(); // 카드가 분배될 때마다 점수 업데이트
    }
    [PunRPC]
    private void HandleGameEnded(string message)
    {
        resultText.text = message;
        turnText.text = "Game Over";
        hitButton.interactable = false;
        standButton.interactable = false;
        restartButton.gameObject.SetActive(true);
        UpdateScores(); // 게임 종료 시 딜러 카드 모두 공개
    }
    [PunRPC]
    private void HandleTurnChanged(PlayerTurn currentPlayer)
    {
        props[RoomInfo.turn.ToString()] = currentPlayer;
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        m_gameLogic.Turn = currentPlayer;
        turnText.text = $"{props[RoomInfo.turn.ToString()]}'s Turn";
    }


    void UpdateScores()
    {
        m_myScoreText.text = $"{m_myNameText.text}: " + m_myBlackJackPlayer.Hand.CalculateValue();
        m_enemyScoreText.text = $"{m_enemyNameText.text}: " + m_enemyBlackJackPlayer.Hand.CalculateValue();
        // 현재 턴 플레이어 강조 또는 UI 업데이트 로직 추가 가능
        // 예: if (blackjackGame.CurrentPlayer == BlackjackGame.PlayerTurn.Player1) { /* Player1 UI 강조 */ }
    }
    Sprite GetCardSprite(string pCardSuit, string pCardRank)
    {
        string spriteName = $"{pCardSuit}_{pCardRank}";
        if (cardSprites.ContainsKey(spriteName))
        {
            return cardSprites[spriteName];
        }
        else
        {
            Debug.LogError($"스프라이트를 찾을 수 없습니다: {spriteName}");
            return null;
        }
    }
    public void Hit()
    {
        foreach(var card in m_enemyBlackJackPlayer.Hand.Cards) {
            Debug.Log(card);
        }
        props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (PhotonNetwork.IsMasterClient && (int)props[RoomInfo.turn.ToString()] == (int)PlayerTurn.Player1)
            photonView.RPC("HitLogic", RpcTarget.All);
        else if (!PhotonNetwork.IsMasterClient && (int)props[RoomInfo.turn.ToString()] == (int)PlayerTurn.Player2)
            photonView.RPC("HitLogic", RpcTarget.All);
    }

    [PunRPC]
    public void HitLogic()
    {
        props = PhotonNetwork.CurrentRoom.CustomProperties;


        if (PhotonNetwork.IsMasterClient && (int)props[RoomInfo.turn.ToString()] == (int)PlayerTurn.Player1)
        {
            m_gameLogic.Hit();
            
        }
        else if (!PhotonNetwork.IsMasterClient && (int)props[RoomInfo.turn.ToString()] == (int)PlayerTurn.Player2)
        {
            m_gameLogic.Hit();
            
        }


        //if (PhotonNetwork.LocalPlayer == m_gameLogic.Player1.Player)
        //{
        //    props[RoomInfo.player1Point.ToString()] = m_myBlackJackPlayer.Hand.CalculateValue();
        //    m_gameLogic.Player1Point = (int)props[RoomInfo.player1Point.ToString()];
        //    props[RoomInfo.player2Point.ToString()] = m_enemyBlackJackPlayer.Hand.CalculateValue();
        //    m_gameLogic.Player2Point = (int)props[RoomInfo.player2Point.ToString()];
        //}
        //else
        //{
        //    props[RoomInfo.player1Point.ToString()] = m_enemyBlackJackPlayer.Hand.CalculateValue();
        //    m_gameLogic.Player1Point = (int)props[RoomInfo.player1Point.ToString()];
        //    props[RoomInfo.player2Point.ToString()] = m_myBlackJackPlayer.Hand.CalculateValue();
        //    m_gameLogic.Player2Point = (int)props[RoomInfo.player2Point.ToString()];
        //}

        UpdateScores();
    }

    public void Stand()
    {
        if (PhotonNetwork.IsMasterClient && (int)props[RoomInfo.turn.ToString()] == (int)PlayerTurn.Player1)
        {
            photonView.RPC("StandLogic", RpcTarget.All);
        }
        else if (!PhotonNetwork.IsMasterClient && (int)props[RoomInfo.turn.ToString()] == (int)PlayerTurn.Player2)
        {
            photonView.RPC("StandLogic", RpcTarget.All);
        }
    }

    [PunRPC]
    public void StandLogic()
    {
        props = PhotonNetwork.CurrentRoom.CustomProperties;

        m_gameLogic.Stand();

        props[RoomInfo.player1IsTurnOn.ToString()] = m_gameLogic.Player1.IsTurnOn;
        props[RoomInfo.player2IsTurnOn.ToString()] = m_gameLogic.Player2.IsTurnOn;

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
}

