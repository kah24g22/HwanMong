using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SoloBlackJack : MonoBehaviour
{
    private SoloBlackJackLogic m_gameLogic;
    // UI 요소 연결
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

    private bool m_isGameOver = false; // 게임 종료 플래그 추가

    [SerializeField] private BlackJackPlayer m_myBlackJackPlayer;
    [SerializeField] private BlackJackPlayer m_enemyBlackJackPlayer;

    [SerializeField] private Image[] m_myLifeHearts; // 내 라이프 하트 UI 배열
    [SerializeField] private Image[] m_enemyLifeHearts; // 적 라이프 하트 UI 배열 

    [SerializeField] Button[] buttons;
    [SerializeField] private TextMeshProUGUI[] m_itemTexts; // 아이템 이름 표시를 위한 TextMeshProUGUI 배열

    private void Awake()
    {
        m_gameLogic = new SoloBlackJackLogic(m_myBlackJackPlayer, m_enemyBlackJackPlayer);

        m_gameLogic.OnCardDealt += CardUpdate;
        m_gameLogic.OnGameEnded += GameEnd;
        m_gameLogic.OnTurnChanged += TurnChanged;

        m_myBlackJackPlayer.OnLifeChanged += (life) => UpdateLifeUI(life, m_myLifeHearts); // 내 라이프 변경 이벤트 구독
        m_enemyBlackJackPlayer.OnLifeChanged += (life) => UpdateLifeUI(life, m_enemyLifeHearts); // 적 라이프 변경 이벤트 구독

        hitButton.onClick.AddListener(() => Hit());
        standButton.onClick.AddListener(() => Stand());
        restartButton.onClick.AddListener(() => StartGame());

        int i = 0;

        foreach (var button in buttons)
        {
            int index = i;
            button.onClick.AddListener(() => UseItem(index));
            if (m_itemTexts != null && index < m_itemTexts.Length)
            {
                m_itemTexts[index].text = ""; // 초기에는 비워둠
            }
            i++;
        }

    }



    void Start()
    {
        cardSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Cards/Cards");

        foreach (Sprite sprite in sprites)
        {
            cardSprites.Add(sprite.name, sprite);
        }
    }

    private void Update()
    {
        if(m_gameLogic.Turn == PlayerTurn.Player2)
        {
            if (m_enemyBlackJackPlayer.Hand.CalculateValue() >= 18)
            {
                Stand();
            }
            else
            {
                Hit();
            }
        }
    }

    public void StartGame()
    {
        StartGameLogic();
    }

    public void StartGameLogic()
    {

        foreach (GameObject card in myCardObjects) Destroy(card);
        myCardObjects.Clear();
        foreach (GameObject card in enemyCardObjects) Destroy(card);
        enemyCardObjects.Clear();
        resultText.text = "";
        turnText.text = "";

        hitButton.interactable = true;
        standButton.interactable = true;

        m_gameLogic.StartGame();

        UpdateItemUI(); // 아이템 UI 업데이트

        m_gameLogic.DealCard(m_myBlackJackPlayer, true);
        m_gameLogic.DealCard(m_enemyBlackJackPlayer, true);
        m_gameLogic.DealCard(m_myBlackJackPlayer, true);
        m_gameLogic.DealCard(m_enemyBlackJackPlayer, false);

        UpdateScores();

        UpdateLifeUI(m_myBlackJackPlayer.Life, m_myLifeHearts); // 게임 시작 시 내 라이프 UI 업데이트
        UpdateLifeUI(m_enemyBlackJackPlayer.Life, m_enemyLifeHearts); // 게임 시작 시 적 라이프 UI 업데이트

        foreach(var p in m_enemyBlackJackPlayer.Hand.Cards)
        {
            Debug.Log(p.suit + p.rank);
        }
    }

    private void UpdateLifeUI(int currentLife, Image[] lifeHearts)
    {
        Debug.Log($"[SoloBlackJack] Updating Life UI. Current Life: {currentLife}, Hearts Array Length: {lifeHearts.Length}");
        for (int i = 0; i < lifeHearts.Length; i++)
        {
            if (i < currentLife)
            {
                lifeHearts[i].gameObject.SetActive(true);
            }
            else
            {
                lifeHearts[i].gameObject.SetActive(false);
            }
        }
    }

    private void CardUpdate(BlackJackPlayer pPlayer, Card card, bool isFaceUp)
    {
        HandleCardDealt(pPlayer, card.suit, card.rank, isFaceUp);
    }
    private void GameEnd(string pMessage)
    {
        HandleGameEnded(pMessage);
    }
    private void TurnChanged(PlayerTurn pCurrentPlayer)
    {
        HandleTurnChanged(pCurrentPlayer);
    }

    private void HandleCardDealt(BlackJackPlayer pPlayer, string pCardSuit, string pCardRank, bool isFaceUp)
    {
        Transform parentTransform;
        List<GameObject> cardObjectsList;

        if (pPlayer == m_myBlackJackPlayer)
        {
            parentTransform = myHandTransform;
            cardObjectsList = myCardObjects;

            GameObject newCardObject = Instantiate(cardPrefab, parentTransform);
            cardObjectsList.Add(newCardObject);

            newCardObject.transform.localPosition = new Vector3(cardSpacing * (cardObjectsList.Count - 1), 0, 0);

            newCardObject.GetComponent<SpriteRenderer>().sprite = GetCardSprite(pCardSuit, pCardRank);
        }
        else if (pPlayer == m_enemyBlackJackPlayer)
        {
            parentTransform = enemyHandTransform;
            cardObjectsList = enemyCardObjects;

            GameObject newCardObject = Instantiate(cardPrefab, parentTransform);
            cardObjectsList.Add(newCardObject);

            newCardObject.transform.localPosition = new Vector3(cardSpacing * (cardObjectsList.Count - 1), 0, 0);

            if (isFaceUp)
            {
                newCardObject.GetComponent<SpriteRenderer>().sprite = GetCardSprite(pCardSuit, pCardRank);
            }
            else
            {
                newCardObject.GetComponent<SpriteRenderer>().sprite = cardSprites["Card_Back"];
            }
            UpdateScores(); // 카드가 분배될 때마다 점수 업데이트
        }
    }

    private void HandleGameEnded(string message)
    {
        resultText.text = message;
        turnText.text = "Game Over";
        hitButton.interactable = false;
        standButton.interactable = false;
        restartButton.gameObject.SetActive(true);
        UpdateScores(); // 게임 종료 시 딜러 카드 모두 공개
        m_isGameOver = true; // 게임 종료 플래그 설정
    }

    private void HandleTurnChanged(PlayerTurn currentPlayer)
    {
        m_gameLogic.Turn = currentPlayer;
        turnText.text = $"{m_gameLogic.Turn}'s Turn";
    }


    void UpdateScores()
    {
        m_myScoreText.text = $"player1: " + m_myBlackJackPlayer.Hand.CalculateValue();
        m_enemyScoreText.text = $"player2: " + m_enemyBlackJackPlayer.Hand.CalculateValue();
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
        HitLogic();
    }

    public void HitLogic()
    {
        if (m_gameLogic.Turn == PlayerTurn.Player1)
        {
            m_gameLogic.Hit();

        }
        else if (m_gameLogic.Turn == PlayerTurn.Player2)
        {
            m_gameLogic.Hit();

        }

        UpdateScores();
    }

    private void UpdateItemUI()
    {
        Debug.Log($"[SoloBlackJack] Updating Item UI for {m_myBlackJackPlayer.name}. Inventory Count: {m_myBlackJackPlayer.Inventory.Count}");
        for (int i = 0; i < m_myBlackJackPlayer.Inventory.Count; i++)
        {
            Debug.Log($"[SoloBlackJack] Item at index {i}: {m_myBlackJackPlayer.Inventory[i]}");
        }

        for (int i = 0; i < m_itemTexts.Length; i++)
        {
            if (i < m_myBlackJackPlayer.Inventory.Count)
            {
                m_itemTexts[i].text = m_myBlackJackPlayer.Inventory[i].ToString();
            }
            else
            {
                m_itemTexts[i].text = "";
            }
        }
    }

    public void Stand()
    {
        if (m_gameLogic.Turn == PlayerTurn.Player1)
        {
            StandLogic();
        }
        else if (m_gameLogic.Turn == PlayerTurn.Player2)
        {
            StandLogic();
        }
    }

    public void StandLogic()
    {
        m_gameLogic.Stand();
    }

    public void UseItem(int pIndex)
    {
        if (pIndex > m_myBlackJackPlayer.Inventory.Count - 1) return;

        UseItemLogic(m_myBlackJackPlayer.Inventory[pIndex], m_myBlackJackPlayer);
    }

    public void UseItemLogic(ItemType itemType, BlackJackPlayer user)
    {
        IItemEffect effect = null;
        switch (itemType)
        {
            case ItemType.Envelope:
                effect = new EnvelopeEffect();
                break;
            case ItemType.Scale:
                effect = new ScaleEffect();
                break;
            case ItemType.Hourglass:
                effect = new HourglassEffect();
                break;
            case ItemType.Match:
                effect = new MatchEffect();
                break;
            case ItemType.Lock:
                effect = new LockEffect();
                break;
            case ItemType.CrystalBall:
                effect = new CrystalBallEffect();
                break;
        }

        if (effect != null)
        {
            Debug.Log(m_gameLogic);
            Debug.Log(user);
            effect.Execute(m_gameLogic, user);
            user.RemoveItem(itemType); // 아이템 사용 후 인벤토리에서 제거
            UpdateItemUI(); // 아이템 사용 후 UI 업데이트
        }
        else
        {
            Debug.LogWarning($"Unknown item type: {itemType}");
        }
    }

}

